
define(['can', 'config'], function(can, Config){
	
	return can.Model({
		
		sendSupportEmail: function(subject, body){
			return $.post('/'+Config.api+'/support/email', {
				subject: subject,
				body: body
			}, 'json');
		},

	}, {});
	
});

