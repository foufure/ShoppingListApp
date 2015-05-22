Feature: US001_GoogleSearch
 As a google public user
    I want to search my blog

@web
Scenario: Type in my blogs'name in google search should return my blog url in search result page
	Given I have 'home' page opened
	And I have entered 'cybtamin codelife' into the text box of which id is 'gbqfq'
	When I press button of which id is 'gbqfb'
	Then I should be navigated to search result page with 'codelife.cybtamin.com' on it
