using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace Terradue.Tep.Urban {
    public class CalvalusUploadClient {

        public string BaseUrl { get; set; }
        public int Timeout { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        public CalvalusUploadClient(string baseurl, string username, string password) {
            this.BaseUrl = baseurl;
            Username = username;
            Password = password;
            Timeout = 3000;
        }

        private HttpWebRequest CreateWebRequest(string url, string method, string username) {
            var request = (HttpWebRequest)WebRequest.Create(url);

            //we increase the Timeout for PUT/POST/DELETE methods
            switch (method) {
                case "PUT":
                case "POST":
                case "DELETE":
                    request.Timeout = this.Timeout * 2;
                    break;
                default:
                    request.Timeout = this.Timeout;
                    break;
            }
            request.Method = method;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Proxy = null;
            request.Credentials = new NetworkCredential(this.Username, this.Password);
            request.PreAuthenticate = true;
            request.Headers.Add("REMOTE_USER", username);

            return request;
        }

        /// <summary>
        /// List installed processor packages
        /// </summary>
        /// <returns>The processors for user.</returns>
        /// <param name="username">User name</param>
        /// <param name="offset">The index of the first result to show</param>
        /// <param name="limit">The maximum count of results</param>
        public List<ProcessorPackage> ProcessorPackageGetList(string username, int offset = 0, int limit = 1000) {
            List<ProcessorPackage> response = new List<ProcessorPackage>();
            var url = string.Format("{0}/processor-packages?offset={1}&limit={2}", this.BaseUrl, offset, limit);
            var request = CreateWebRequest(url, "GET", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<List<ProcessorPackage>>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
            return response;
        }

        public void ProcessorPackageUpload(string username, string filepath){
            var url = string.Format("{0}/processor-packages", this.BaseUrl);

            FileUpload(url, username, filepath);
            File.Delete(filepath);
        }

        private void FileUpload(string url, string username, string filepath){

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.Credentials = new NetworkCredential(this.Username, this.Password);
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("REMOTE_USER", username);
            httpWebRequest.ServicePoint.Expect100Continue = false;

            var filename = filepath.Substring(filepath.LastIndexOf("/") + 1);
            var pre = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", boundary, filename, "application/zip");
            var post = string.Format("\r\n--{0}--\r\n", boundary);

            Task task2 = null;
            var task = Task.Factory.FromAsync(httpWebRequest.BeginGetRequestStream, httpWebRequest.EndGetRequestStream, httpWebRequest).ContinueWith(
                req => {
                    using (var requestStream = req.Result) {

                        var preByte = System.Text.Encoding.Default.GetBytes(pre);
                        requestStream.Write(preByte, 0, preByte.Length);

                        using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read)) {
                            byte[] buffer = new byte[2048];
                            int bytesRead;
                            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0) {
                                requestStream.Write(buffer, 0, bytesRead);
                            }
                        }

                        var postByte = System.Text.Encoding.Default.GetBytes(post);
                        requestStream.Write(postByte, 0, postByte.Length);
                        requestStream.Flush();

                        task2 = Task.Factory.FromAsync(httpWebRequest.BeginGetResponse, httpWebRequest.EndGetResponse, null).ContinueWith(
                            resp => {
                                using (var streamReader = new StreamReader(resp.Result.GetResponseStream())) {
                                    string result = streamReader.ReadToEnd();
                                }
                            });
                    }
                });
            task.Wait();
            try {
                task2.Wait();
            } catch (Exception e) {
                var toto = e;
            }
        }

        /// <summary>
        /// Get information about processor package
        /// </summary>
        /// <returns>information about processor package</returns>
        /// <param name="name">The processor name and version</param>
        /// <param name="username">User name.</param>
        public ProcessorPackage ProcessorPackageGetInfo(string name, string username) {
            ProcessorPackage response = null;
            var url = string.Format("{0}/processor-packages/{1}", this.BaseUrl, name);
            var request = CreateWebRequest(url, "GET", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<ProcessorPackage>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Delete a processor package.
        /// </summary>
        /// <returns>The processor package.</returns>
        /// <param name="name">The processor name and version.</param>
        /// <param name="username">User name.</param>
        public void ProcessorsPackageDelete(string name, string username) {
            string response = null;
            var url = string.Format("{0}/processor-packages/{1}", this.BaseUrl, name);
            var request = CreateWebRequest(url, "DELETE", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<string>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve content of processor package as zip file
        /// </summary>
        /// <returns>The processor package content.</returns>
        /// <param name="name">The processor name and version.</param>
        /// <param name="username">User name.</param>
        public object ProcessorPackageGetContent(string name, string username) {
            var url = string.Format("{0}/processor-packages/{1}/content", this.BaseUrl, name);

            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(this.Username, this.Password);
            webClient.Headers.Add("REMOTE_USER", username);
            var response = webClient.DownloadData(url);

            return response;
        }

        /// <summary>
        /// Lists the files of a processor package
        /// </summary>
        /// <returns>The processor package content.</returns>
        /// <param name="name">The processor name and version.</param>
        /// <param name="username">User name.</param>
        public List<ProcessorPackageFile> ProcessorPackageGetFiles(string name, string username) {
            List<ProcessorPackageFile> response = null;
            var url = string.Format("{0}/processor-packages/{1}/files", this.BaseUrl, name);
            var request = CreateWebRequest(url, "GET", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<List<ProcessorPackageFile>>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Adds or replaces a single file of a processor package
        /// </summary>
        /// <returns>The processor package file.</returns>
        /// <param name="name">The processor name and version.</param>
        /// <param name="username">User name.</param>
        public void ProcessorPackageFileUpdate(string name, string username, Stream stream) {
            var url = string.Format("{0}/processor-packages/{1}/files", this.BaseUrl, name);
            var request = CreateWebRequest(url, "POST", username);
            request.KeepAlive = true;
            request.ContentType = "multipart/form-data";

            using (var s = request.GetRequestStream()) {
                stream.CopyTo(s);
                using (var response = (HttpWebResponse)request.GetResponse()) {
                    using (var streamReader = new StreamReader(response.GetResponseStream())) {
                        string result = streamReader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the given file's metadata
        /// </summary>
        /// <returns>The processor package content.</returns>
        /// <param name="name">The processor name and version.</param>
        /// <param name="filename">The filename to retrieve the metadata from.</param>
        /// <param name="username">User name.</param>
        public ProcessorPackageFile ProcessorPackageGetFileMetadata(string name, string filename, string username) {
            ProcessorPackageFile response = null;
            var url = string.Format("{0}/processor-packages/{1}/files/{2}", this.BaseUrl, name, filename);
            var request = CreateWebRequest(url, "GET", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<ProcessorPackageFile>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Delete a file from a processor package
        /// </summary>
        /// <param name="name">The processor name and version.</param>
        /// <param name="filename">The filename to delete.</param>
        /// <param name="username">User name.</param>
        public void ProcessorPackageDeleteFile(string name, string filename, string username) {
            string response = null;
            var url = string.Format("{0}/processor-packages/{1}/files/{2}", this.BaseUrl, name, filename);
            var request = CreateWebRequest(url, "DELETE", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<string>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve content of processor file as zip file
        /// </summary>
        /// <returns>The processor package content.</returns>
        /// <param name="name">The processor name and version.</param>
        /// <param name="filename">The filename to retrieve the metadata from.</param>
        /// <param name="username">User name.</param>
        public object ProcessorPackageGetFileContent(string name, string filename, string username) {
            var url = string.Format("{0}/processor-packages/{1}/files/{2}/content", this.BaseUrl, name, filename);

            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential(this.Username, this.Password);
            webClient.Headers.Add("REMOTE_USER", username);
            var response = webClient.DownloadData(url);
            return response;
        }

        /// <summary>
        /// Lists the shapefiles.
        /// </summary>
        /// <returns>The shapefiles.</returns>
        /// <param name="username">User name.</param>
        public List<ShapefilePackage> ShapefilesGetList(string username) {
            List<ShapefilePackage> response = null;
            var url = string.Format("{0}/shapefiles", this.BaseUrl);
            var request = CreateWebRequest(url, "GET", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<List<ShapefilePackage>>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Uploads the Shapefile
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="data">Data.</param>
        /// <param name="filename">Filename.</param>
        public void ShapefileUpload(string username, string filepath) {
            var url = string.Format("{0}/shapefiles", this.BaseUrl);

            FileUpload(url, username, filepath);
            File.Delete(filepath);
        }

        /// <summary>
        /// Gets the shapefile info.
        /// </summary>
        /// <returns>The shapefile info.</returns>
        /// <param name="filename">The filename of the shapefile to get metainfo for.</param>
        /// <param name="username">User name.</param>
        public ShapefilePackage ShapefileGetInfo(string filename, string username) {
            ShapefilePackage response = null;
            var url = string.Format("{0}/shapefiles/{1}", this.BaseUrl, filename);
            var request = CreateWebRequest(url, "GET", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<ShapefilePackage>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
            return response;
        }


        /// <summary>
        /// Deletes the shapefile info.
        /// </summary>
        /// <returns>The shapefile info.</returns>
        /// <param name="filename">The filename of the shapefile to delete.</param>
        /// <param name="username">User name.</param>
        public void ShapefileDelete(string filename, string username) {
            string response = null;
            var url = string.Format("{0}/shapefiles/{1}", this.BaseUrl, filename);
            var request = CreateWebRequest(url, "DELETE", username);

            using (var httpResponse = (HttpWebResponse)request.GetResponse()) {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    string result = streamReader.ReadToEnd();
                    try {
                        response = JsonSerializer.DeserializeFromString<string>(result);
                    } catch (Exception e) {
                        throw e;
                    }
                }
            }
        }

    }
}
