

define({
	template: 'mustache',
	errorNotify: true,
	errorLog: false,
	mainContainer: '#mainContainer',
	pagesViewsPath: '/pages/',
	pageError: "modules/pages/views/error.html",
	api: "t2api",
	siteName: "TEP Urban",
	tumblrBlogLink: "http://esa-official.tumblr.com/",
	tumblrApiKey: "fuiKNFp9vQFvjLNvx4sUwti4Yb5yGutBN4Xh10LXZhhRKjWlV4",
	
	menuUrl: "/config/menu.json",
	
	enableLoginPolling: true,
	enableAccounting: true,
	
	baseControl: {
		siteName: 'TEP Urban',
		contactUsUrl: 'mailto:urban-tep@esa.int',
		supportUrl: 'mailto:support@terradue.com',
		errorImageUrl: '/styles/img/error.png'
	},
	
	modules: {
		pages: {
			discourseBlogEnabled: true,
			discourseCategory: 9
		},
		
		blog: {
			discourseBlogEnabled: true,
			discourseCategory: 9
		},
		
		login: {
			siteName: 'TEP Urban',
			contactAddress: 'urban-tep@esa.int',
			docsUrl: 'http://urban-tep.github.io/documentation/',
			supportUrl: 'https://utepredmine.it4i.cz/redmine/',
			hasSupport: true,
			loginPollingTime: 60*1000 // 1 minute
		},
		
		settings: {
			subContainer: "#subContainer",
			showPagination: false,
			stepScoreUsage: 50,
			nbStarsUsage: 5,
			messagesUsage: ["Newbie","Rookie","Beginner","Intermediate","Advanced","Expert"]
		}
	}
	
});