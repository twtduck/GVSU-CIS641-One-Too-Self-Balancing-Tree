# Overview

This application is a desktop client for Google Calendar that extends part of its functionality. In extending an already complex product, it can be said with relative certainty that this project is going to be complex itself. By using a software requirements specification, a plan for dividing the effort into digestible chunks of work  can be made. The requirements specification will outline all of the criteria the finished product must meet, and the expected interactions between actors and the system.

# Functional Requirements

1. Calendar Viewer
   1. The user shall click a button to sync the current week
   2. The user shall click the next/previous week buttons to change viewed week
   3. The view shall display event details when the event is clicked 
   4. The view shall show all the events for the week at the correct day and time
2. Backlog
   1. The user shall add events to the backlog like a simple list
   2. The user shall assign time durations to events in the backlog
   3. The user shall move events from the backlog to the calendar
3. Google Calendar Sync
   1. The user shall log in using a web browser authentication prompt
   2. When requested, the calendar shall push new events to the online calendar
   3. The locally stored calendar shall pull any event changes/new events from the online calendar

# Non-Functional Requirements

1. The application shall run on a windows PC
2. The application shall start in less than three seconds
3. Internet-dependent operations shall handle errors gracefully
4. The calendar view will visually resemble a Google Calendar web view
