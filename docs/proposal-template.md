Team name: One Too (Self Balancing) Tree

Team members: Me (Thomas)

# Introduction

The "One Too (Self Balancing) Tree" calendar app (name is WIP) is to help make organizing a personal calendar just a little easier. Users can put items into lists (referred to as "backlogs") and then later assign time requirements and a time for it. The app will then sync the event to a connected Google calendar instance.

Possible avenues for extension include categorizing events, recommending events for time slots, and recommending time durations for events. Additionally, there may be a future for editing events, adding recurring events, and web and mobile app frontend implementations. 

# Anticipated Technologies

My plan for the initial implementation is to use .NET 6/C# to write the backend. For a GUI, I plan to use Windows Presentation Foundation. However, I plan to do some research into other options, particularly MAUI and Xamarin Forms. Development will be done on Windows using JetBrains Rider and Git for version control.  

# Method/Approach

My plan for this project is to start by putting together a simple calendar viewer for the current week. Once that is completed, a logical next step would be allowing the user to create events, view details of events and to page between weeks. Finally, adding a "backlogs" of events would be the last increment for the project. 

# Estimated Timeline

Using the scrum process as inspiration, I plan to complete the project increments in 2-week sprints.  
- Weeks of 9/19 and 9/26
  - Do research into technologies being used
  - Become familiar with the Google calendar API
  - Add all the key PBIs to the project backlog
- Weeks of 10/3 and 10/10:
  - Complete initial implementation of calendar viewer
- Weeks of 10/17 and 10/24:
  - Implement event creation
  - Add a view for viewing event details
- Weeks of 10/31 and 11/7:
  - Support event editing
  - Add backlogs and the ability to move items between backlogs and the calendar
- Weeks of 11/14 and 11/21
  - Add a view for seeing and editing event details
- Weeks of 11/28 and 12/5
  - Deploy project to the public
- Week of 12/12
  - Prepare presentation of project

# Anticipated Problems

One problem I anticipate is trouble with the Google calendar API. While getting it to work for development seems easy enough, it is not very clear what may be required in order to publicly deploy the project for anyone to use. Another problem is it is hard to anticipate what challenges may be encountered along the way. As a consequence, the above schedule does seem to "thin out" in work per week. This is so that any bugs added in earlier increments, PBIs left uncompleted, and complications have time to be contended with. If things go more smoothly than anticipated, there are a number of avenues for extension, some of which are detailed above. Finally, I will be using .NET 6, which has been announced to be "production ready" but still hasn't been officially released and is still only available as a preview. There may be some bugs in this version of .NET that could force development to roll back to an older version.  