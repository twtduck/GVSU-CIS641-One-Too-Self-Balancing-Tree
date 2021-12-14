# Overview

This application is a desktop client for Google Calendar that extends part of its functionality. In extending an already complex product, it can be said with
relative certainty that this project is going to be complex itself. By using a software requirements specification, a plan for dividing the effort into
digestible chunks of work  can be made. The requirements specification will outline all of the criteria the finished product must meet, and the expected
interactions between actors and the system.

# Software Requirements

## Functional Requirements

### Calendar Viewer

|  ID   | Requirement                                                                            |
|:-----:|:---------------------------------------------------------------------------------------|
|  FR1  | The user shall click a button to sync the current week                                 |
|  FR2  | The user shall click the next/previous week buttons to change viewed week              |
|  FR3  | The view shall display event details when the event is clicked                         | 
|  FR4  | The view shall show all the events for the week at the correct day and time            |
|  FR5  | The view shall show the current time and day, and change the appearance of past events |
|  FR6  | The user shall be able to drag and drop events to move them on the calendar            |
|  FR7  | The user shall be able to delete an event                                              |

### Backlog

|   ID    | Requirement                                                              |
|:-------:|:-------------------------------------------------------------------------|
|   FR8   | The backlog shall store events not assigned a time                       |
|   FR9   | The user shall be able to move an event from the calendar to the backlog |
|  FR10   | The user shall be able to move an event from the backlog to the calendar |

### Google Calendar Sync

|  ID  | Requirement                                                                                                                                     |
|:----:|:------------------------------------------------------------------------------------------------------------------------------------------------|
| FR11 | The user shall log in using a web browser authentication prompt                                                                                 |
| FR12 | When requested, the calendar shall push new events to the online calendar                                                                       |

### Event editor

|  ID  | Requirement                                                                                                              |
|:----:|:-------------------------------------------------------------------------------------------------------------------------|
| FR13 | The user shall be able to change the end time, start time, title, description, color, location, and calendar of an event |

## Non-Functional Requirements

### User Interface/Experience

|  ID  | Requirement                                                         |
|:----:|:--------------------------------------------------------------------|
| NFR1 | The application shall run on a windows PC                           |
| NFR2 | The application shall start in less than three seconds              |
| NFR3 | Internet-dependent operations shall handle errors gracefully        |
| NFR4 | The calendar view will visually resemble a Google Calendar web view |

# Change Management Plan

Bringing this tool into your organization is meant to be simple and easy. For training, all that should be needed is a basic
knowledge of Google Calendar and maybe a quick perusal of the README.md file. Organizations could also request a short webinar (less than an hour long)
to get a better understanding of the application and its uses. To make distribution and integration with people's existing systems easier, a self-contained
installer (including the required runtime and program files) should be developed. The github repository will remain public for the lifetime of the project, 
as it will not only be used for releasing updates, but also to give users a place they may report issues and get help or updates that fix them.

# Traceability Links

| Feature         | Requirement | Activity Diagram                                                                                                                                            | Use Case                                                                                                                              | Class Diagram                                                                         | Object Diagram                                                                                        | State Diagram                                                                       | Sequence Diagram                                                                                                            |
|:----------------|:------------|:------------------------------------------------------------------------------------------------------------------------------------------------------------|:--------------------------------------------------------------------------------------------------------------------------------------|:--------------------------------------------------------------------------------------|:------------------------------------------------------------------------------------------------------|:------------------------------------------------------------------------------------|:----------------------------------------------------------------------------------------------------------------------------|
| Calendar Viewer | FR1         | TBD                                                                                                                                                         | [artifacts/functional-models/UseCase_Refresh.docx](../artifacts/functional-models/UseCase_Refresh.docx)                               | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | [docs/SequenceDiagram_InitializeAndSynchronizeViewer.drawio](../docs/SequenceDiagram_InitializeAndSynchronizeViewer.drawio) |
| Calendar Viewer | FR2         | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Calendar Viewer | FR3         | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | [docs/StateMachine_UserInterface.drawio](../docs/StateMachine_UserInterface.drawio) | TBD                                                                                                                         |
| Calendar Viewer | FR4         | [artifacts/functional-models/ActivityDiagram_CalendarViewer.drawio](../artifacts/functional-models/ActivityDiagram_CalendarViewer.drawio)                   | [artifacts/functional-models/UseCase_CalendarViewer.drawio](../artifacts/functional-models/UseCase_CalendarViewer.drawio)             | [docs/ClassDiagram_CalendarViewer.drawio](../docs/ClassDiagram_CalendarViewer.drawio) | [docs/ObjectDiagram_WeekCalendarViewModel.drawio](../docs/ObjectDiagram_WeekCalendarViewModel.drawio) | TBD                                                                                 | TBD                                                                                                                         |
| Calendar Viewer | FR5         | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Calendar Viewer | FR6         | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Calendar Viewer | FR7         | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Backlog         | FR8         | [artifacts/functional-models/ActivityDiagram_Backlog.drawio](../artifacts/functional-models/ActivityDiagram_Backlog.drawio)                                 | [artifacts/functional-models/UseCase_Backlog.drawio](../artifacts/functional-models/UseCase_Backlog.drawio)                           | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Backlog         | FR9         | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Backlog         | FR10        | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Synchronization | FR11        | TBD                                                                                                                                                         | TBD                                                                                                                                   | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Synchronization | FR12        | [artifacts/functional-models/ActivityDiagram_CalendarSynchronization.drawio](../artifacts/functional-models/ActivityDiagram_CalendarSynchronization.drawio) | [artifacts/functional-models/UseCase_CalendarSynchronizer.drawio](../artifacts/functional-models/UseCase_CalendarSynchronizer.drawio) | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |
| Event Editor    | FR13        | TBD                                                                                                                                                         | [artifacts/functional-models/UseCase_EventDetails.drawio](artifacts/functional-models/UseCase_EventDetails.drawio)                    | TBD                                                                                   | TBD                                                                                                   | TBD                                                                                 | TBD                                                                                                                         |

# Software artifacts
- [Midterm Project Presentation Materials](../docs/midterm-presentation)
- [Final Project Presentation Materials](../docs/final-presentation)
- [Meeting Notes](../meetings)