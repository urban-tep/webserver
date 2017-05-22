
define([
	'jquery',
	'can',
	'utils/baseControl',
	'config',
	'utils/helpers',
	'modules/offercontent/models/offercontent',
	'moment'
], function($, can, BaseControl, Config, Helpers, OfferContentModel){
	
	var OfferContentControl = BaseControl({
		defaults: { fade: 'slow' },
	}, {
		init: function(element, options){
			this.isLoginPromise = App.Login.isLoggedDeferred;
		},
		
		index: function(data){
			var self = this;
			var data = new can.Observe({});
				
			this.element.html(can.view("modules/offercontent/views/offercontent.html", data));
			
			this.isLoginPromise.then(function(user){
				data.attr('user', user);
			});
		},

		'.submitForm click': function(el){
			var subject = el.closest('form').data('subject');
			var body = "";
			var inputs = Helpers.retrieveDataFromForm(el.closest('form'));
			$.each(inputs, function(key, value){
				body += key+': '+value;
				body += "\n";
			});
			OfferContentModel.sendSupportEmail(subject, body);
			return false;
		},

		'.goToHash click': function(el){
			Helpers.scrollToHash(el.data('id'),true);
			return false;
		}
		
	});
	
	return new OfferContentControl(Config.mainContainer, {});
	
});
