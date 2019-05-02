
define(['can', 'config'], function(can, Config){
	
	return can.Model({

		getProcessorPackages: function(){
            return $.ajax('/'+Config.api+'/utep/processor-packages', {
                type : "GET",
                dataType : "json"
            });
        },

        getProcessorPackageContent: function(id){
			return $.ajax({
				type: 'POST',
				url: '/'+Config.api+'/utep/processor-packages/content',
				data: {
					name: id
				},
				dataType: 'json',
			});
		},

        getProcessorPackageFiles: function(id){
			return $.ajax({
				type: 'GET',
				url: '/'+Config.api+'/utep/processor-packages/files',
				data: {
					name: id
				},
				dataType: 'json',
			});
		},

		removeProcessorPackage: function(id){
			return $.ajax({
				type: 'DELETE',
				url: '/'+Config.api+'/utep/processor-packages',
				data: {
					name: id
				},
				dataType: 'json',
			});
		},

		removeProcessorPackageFile: function(id, filename){
			return $.ajax({
				type: 'DELETE',
				url: '/'+Config.api+'/utep/processor-packages/files',
				data: {
					name: id,
					filename: filename
				},
				dataType: 'json',
			});
		},

		getShapefiles: function(){
            return $.ajax('/'+Config.api+'/utep/shapefiles', {
                type : "GET",
                dataType : "json"
            });
        },

        removeShapefile: function(id){
			return $.ajax({
				type: 'DELETE',
				url: '/'+Config.api+'/utep/shapefiles',
				data: {
					name: id
				},
				dataType: 'json',
			});
		}

	}, {});
	
});

