
define([
	'jquery',
	'can',
	'utils/baseControl',
	'config',
	'utils/helpers',
	'modules/upload/models/upload',
	'bootbox',
	'dropzone',
	'moment',
	'messenger'
], function($, can, BaseControl, Config, Helpers, UploadModel, bootbox, Dropzone){
	
	var UploadControl = BaseControl({
		defaults: { fade: 'slow' },
	}, {
		init: function(element, options){
			this.isLoginPromise = App.Login.isLoggedDeferred;
		},
		
		index: function(data){
			this.data = new can.Observe();
			var data = this.data;
			data.attr('loadingProcessors',true);
			data.attr('loadingShapefiles',true);

			this.loadPackages();
			this.loadShapefiles();
	
			this.element.html(can.view("modules/upload/views/upload.html", data));
			
			this.isLoginPromise.then(function(user){
				data.attr('user', user);
			});

			this.initDropzone();
		},

		initDropzone: function(){
			var self = this;
			var $dropzoneProcessor = this.element.find('.dropzoneProcessor');
			this.dropzoneProcessor = new Dropzone($dropzoneProcessor.get(0), {				
				acceptedFiles: '.zip',
				maxFilesize: 10,
				addRemoveLinks: true,
				uploadMultiple: false,
				autoProcessQueue: false,
				createImageThumbnails: false,
				url: 't2api/utep/processor-packages',
				previewTemplate: "<div class=\"dz-preview dz-file-preview\">\n  <div class=\"dz-details\">\n    <div class=\"dz-size\"><span data-dz-size></span></div>\n    <div class=\"dz-filename\"><span data-dz-name></span></div>\n  </div>\n  <div class=\"dz-progress\"><span class=\"dz-upload\" data-dz-uploadprogress></span></div>\n  <div class=\"dz-error-message text-danger\"><span data-dz-errormessage></span></div>\n  </div>",
				
				init: function() {
					var $submitButton = document.getElementById("submitUploadProcessor"); 
					$submitButton.setAttribute('disabled',true); 
					$('#submitUploadProcessor').click(function() {
						self.dropzoneProcessor.processQueue(); // Tell Dropzone to process all queued files.
					});
					
					// show button when a file is added
					this.on('addedfile', function(file) {
						// remove previous file
						if (this.files.length>1)
							this.removeFile(this.files[0]);
						
						if (file.size<=10*1024*1024 && file.type == "application/zip")
							$submitButton.removeAttribute('disabled');
						else
							$submitButton.setAttribute('disabled',true);
					});
					
					// hide submit button when a file is removed
					this.on('removedfile', function(file) {
						$submitButton.setAttribute('disabled',true);
					});
					
					this.on('success', function(file, package){
						// update the values on the view
						self.loadPackages();
						
						// remove all files
						this.removeAllFiles();
					});
					
				},

				error: function(file, message) {
			        var node, _i, _len, _ref, _results;
			        if (file.previewElement) {
			          file.previewElement.classList.add("dz-error");
			          if (typeof message !== "String" && message.error) {
			            message = message.error;
			          }
			          else if(message.ResponseStatus) {
  						message = message.ResponseStatus.Message;
					  }
			          _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
			          _results = [];
			          for (_i = 0, _len = _ref.length; _i < _len; _i++) {
			            node = _ref[_i];
			            _results.push(node.textContent = message);
			          }
			          return _results;
			        }
			      }
				
			});

			var $dropzoneShapefile = this.element.find('.dropzoneShapefile');
			this.dropzoneShapefile = new Dropzone($dropzoneShapefile.get(0), {				
				acceptedFiles: '.zip',
				maxFilesize: 10,
				addRemoveLinks: true,
				uploadMultiple: false,
				autoProcessQueue: false,
				createImageThumbnails: false,
				url: 't2api/utep/shapefiles',
				previewTemplate: "<div class=\"dz-preview dz-file-preview\">\n  <div class=\"dz-details\">\n    <div class=\"dz-size\"><span data-dz-size></span></div>\n    <div class=\"dz-filename\"><span data-dz-name></span></div>\n  </div>\n  <div class=\"dz-progress\"><span class=\"dz-upload\" data-dz-uploadprogress></span></div>\n  <div class=\"dz-error-message text-danger\"><span data-dz-errormessage></span></div>\n  </div>",
				
				init: function() {
					var $submitButton = document.getElementById("submitUploadShapefile"); 
					$submitButton.setAttribute('disabled',true); 
					$('#submitUploadShapefile').click(function() {
						self.dropzoneShapefile.processQueue(); // Tell Dropzone to process all queued files.
					});
					
					// show button when a file is added
					this.on('addedfile', function(file) {
						// remove previous file
						if (this.files.length>1)
							this.removeFile(this.files[0]);
						
						if (file.size<=10*1024*1024 && file.type == "application/zip")
							$submitButton.removeAttribute('disabled');
						else
							$submitButton.setAttribute('disabled',true);
					});
					
					// hide submit button when a file is removed
					this.on('removedfile', function(file) {
						$submitButton.setAttribute('disabled',true);
					});
					
					this.on('success', function(file, package){
						// update the values on the view
						self.loadShapefiles();
						
						// remove all files
						this.removeAllFiles();
					});
					
				},

				error: function(file, message) {
			        var node, _i, _len, _ref, _results;
			        if (file.previewElement) {
			          file.previewElement.classList.add("dz-error");
			          if (typeof message !== "String" && message.error) {
			            message = message.error;
			          }
			          else if(message.ResponseStatus) {
  						message = message.ResponseStatus.Message;
					  }
			          _ref = file.previewElement.querySelectorAll("[data-dz-errormessage]");
			          _results = [];
			          for (_i = 0, _len = _ref.length; _i < _len; _i++) {
			            node = _ref[_i];
			            _results.push(node.textContent = message);
			          }
			          return _results;
			        }
			      }
				
			});
		},

		loadPackages: function(){
			var data = this.data;
			data.attr('loadingProcessors',true);
			UploadModel.getProcessorPackages().then(function(processors){
				if(processors){
					$.each(processors, function(i,p){
						p.id = p.name + "-" + p.version;
					});
					data.attr('processor',processors);
				} else {
					data.attr('processor',[]);
				}
				data.attr('loadingProcessors',false);
            }).fail(function(xhr){
            	data.attr('loadingProcessors',false);
				bootbox.alert('Fail to load the packages:<br/>' + Helpers.getErrMsg(xhr, 'generic error'));
			});
		},

		loadShapefiles: function(){
			var data = this.data;
			data.attr('loadingShapefiles',true);
			UploadModel.getShapefiles().then(function(shapefiles){
				if(shapefiles){
					$.each(shapefiles, function(i,p){
						p.id = p.name + "-" + p.version;
					});
					data.attr('shapefiles',shapefiles);
				} else {
					data.attr('shapefiles',[]);
				}
				data.attr('loadingShapefiles',false);
            }).fail(function(xhr){
            	data.attr('loadingShapefiles',false);
				bootbox.alert('Fail to load the shapefile:<br/>' + Helpers.getErrMsg(xhr, 'generic error'));
			});
		},

        '.showProcessorFiles click': function(el){
        	var data = this.data;

        	var id = el.parent().data('id');
        	$.each(data.processor, function(i,p){
        		if (p.id == id) p.attr('loading',true);
        	});
        	UploadModel.getProcessorPackageFiles(id).then(function(files){
                $.each(data.processor, function(i,p){
	        		if (p.id == id){
	        			p.attr('files',files);
	        			p.attr('loading',false);
	        		}
	        	});
            }).fail(function(xhr){
				bootbox.alert('Fail to load the package files:<br/>' + Helpers.getErrMsg(xhr, 'generic error'));
			});
            return false;
        },

        '.hideProcessorFiles click': function(el){
        	var data = this.data;
        	var id = el.parent().data('id');
        	$.each(data.processor, function(i,p){
        		if (p.id == id) p.attr('files',[]);
        	});
            return false;
        },

        '.downloadProcessorContent click': function(el){
		    a.href = 'data:text/csv;charset=utf-8,' + encodeURI(csvContent);
		    a.download = communityId + '-users.csv';
		    a.click();
		    URL.revokeObjectURL(a.href);
  			a.remove();
		    return false;
        },

        '.removeProcessor click': function(el){
        	var data = this.data;
        	var self = this;
        	var id = el.parent().data('id');
        	bootbox.confirm('Are you sure you want to remove the processor package <b>'+id+'</b>?', function(confirm){
				if (confirm){
					UploadModel.removeProcessorPackage(id).then(function(){
						self.loadPackages();
					}).fail(function(xhr){
						bootbox.alert('Fail to remove the package:<br/>' + Helpers.getErrMsg(xhr, 'generic error'));
					});
				}
			});
            return false;
        },

        '.removeShapefile click': function(el){
        	var data = this.data;
        	var self = this;
        	var id = el.parent().data('id');
        	bootbox.confirm('Are you sure you want to remove the shapefile <b>'+id+'</b>?', function(confirm){
				if (confirm){
					UploadModel.removeShapefile(id).then(function(){
						self.loadShapefiles();
					}).fail(function(xhr){
						bootbox.alert('Fail to remove the shapefile:<br/>' + Helpers.getErrMsg(xhr, 'generic error'));
					});
				}
			});
            return false;
        },

        '.removeProcessorFile click': function(el){
        	var data = this.data;
        	var self = this;
        	var id = el.data('id');
        	var filename = el.data('filename');
        	bootbox.confirm('Are you sure you want to remove the file <b>' + filename + '</b> from processor package <b>'+id+'</b>?', function(confirm){
				if (confirm){
					UploadModel.removeProcessorPackageFile(id).then(function(){
						UploadModel.getProcessorPackageFiles(id).then(function(files){
			                $.each(data.processor, function(i,p){
				        		if (p.id == id){
				        			p.attr('files',files);
				        			p.attr('loading',false);
				        		}
				        	});
			            }).fail(function(xhr){
							bootbox.alert('Fail to load the package files:<br/>' + Helpers.getErrMsg(xhr, 'generic error'));
						});
					}).fail(function(xhr){
						bootbox.alert('Fail to remove the package:<br/>' + Helpers.getErrMsg(xhr, 'generic error'));
					});
				}
			});
            return false;
        },

        '.refreshPackages click': function(el){
        	this.loadPackages();
			return false;
        },


        '.refreshShapefiles click': function(el){
        	this.loadShapefiles();
			return false;
        }
		
	});
	
	return new UploadControl(Config.mainContainer, {});
	
});
