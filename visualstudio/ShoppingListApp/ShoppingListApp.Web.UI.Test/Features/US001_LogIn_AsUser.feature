Feature: US001_LogIn_AsUser
 As a shoppinglist app user
 I want to log into the website
 So that I can create shoppinglists

@web
Scenario: Log in as a valid user on the shoppinglist app opens the home page
	Given I have '/' page opened
	Given I have pressed button 'LogOn'
	And I have entered 'shoppinglistappharbor@gmail.com' into the text box of which id is 'Email'
	And I have entered 'cambrai19821981siltzheim' into the text box of which id is 'Passwd'
	When I press button of which name is 'signIn'
	Then I should be navigated to home page with 'Log Off' on it
