define([
	'jquery',
	'can',
	'underscore',
	'app',
	'modules/pages/controllers/pages',
	'underscorestring'
], function($, can, _, App, Pages){
	// merge string plugin to underscore namespace
	_.mixin(_.str.exports());
	
	var Router = can.Control ({
		init: function () {},

		// ROUTES
		'route': 'index',
		'contact route': 'contact',
		
		'pages/:topPage/:id route': 'pages',
		'pages/:topPage/:id&selector route': 'pages',
		'pages/:id&:selector route': 'pages',
		'pages/:id route': 'pages',
		
		':controller/:action/:id route': 'dispatch',
		':controller/:action route': 'dispatch',
		':controller route': 'dispatch',

		// ACTIONS
		
		// index
		index: function () {
			Pages.index({ fade: false });
		},
		
		// contact route action (static page)
		contact: function () {
			Pages.contact();
		},
		
		// pages route action (dynamic pages)
		pages: function (data) {
			Pages.load(data);
		},

		// rest route actions
		dispatch: function (data) {
			var me = this;
			
			//SCRUB URL PARAMS IF APPLICABLE
			var ControllerName = _.capitalize (data.controller);
			//CONVERT URL PARAM TO ACTION NAMING CONVENTION
			var actionName = data.action
			? data.action.charAt(0).toLowerCase() + _.camelize(data.action.slice(1))
					: 'index'; // default to index action
			
			//DYNAMICALLY REQUEST CONTROLLER AND PERFORM ACTION
			App.loadController (ControllerName, function (controller) {
				//CALL ACTION WITH PARAMETERS IF APPLICABLE
				if (controller && controller[actionName])
					controller[actionName](data);
			//		TODO: FIX BUG, ONLY WORKS ON FIRST HIT
			//		DUE TO HOW REUIREJS ERROR EVENT WORKS
				else Pages.errorView({}, null, "Controller not found: "+ControllerName);
			}, function(err){
				Pages.errorView({}, null, "Controller not found: "+ControllerName);
				window.err=err;
			});
			
		}
	});
	
	return {
		init: function() {
			var self = this;
			this.currentHash = null;
			
			// route on document.ready
			$(function() {
				// deactivate routing until it's not instantiated
				//can.route.ready(false);

				// init
				new Router(document);

				// events
				can.route.bind('change', function(ev, attr, how, newVal, oldVal) {
					
					var hash = _.ltrim(window.location.hash, '#!');
						currentHash = self.currentHash;
						
					if (hash!=currentHash){
						// hash changed, update the menu
						Pages.initMenu();
						self.currentHash = hash;
					}
				});

				// activate routing
				can.route.ready();
			});
		}
	};
});

