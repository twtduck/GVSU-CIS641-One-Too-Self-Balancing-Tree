# One Too (Self Balancing) Tree

The "One Too (Self Balancing) Tree" calendar app is to help make organizing a personal calendar just a little easier. Users can put items into lists (referred to as "backlogs") and then later assign time requirements and a time for it. The app will then sync the event to a connected Google calendar instance.

## Team Members and Roles

* Thomas Woltjer (all)

## Team Documents

- [Project Proposal](docs/project-proposal.md)
- [Software Requirements Specification - Final (Contains artifact links)](docs/software_requirements_specification_final.md)

## Meeting Minutes

- [21 Sept 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-09-21.md)
- [28 Sept 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-09-28.md)
- [2 Oct 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-10-2.md)
- [11 Oct 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-10-11.md)
- [19 Oct 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-10-19.md)
- [27 Oct 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-10-27.md)
- [3 Nov 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-11-3.md)
- [10 Nov 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-11-10.md)
- [16 Nov 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-11-16.md)
- [2 Dec 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-12-2.md)
- [8 Dec 2021](meetings/GVSU-CIS641-OneTooSelfBalancingTree-2021-12-8.md)

## Prerequisites

Install the .NET 6 SDK. It is available for download from https://dotnet.microsoft.com/en-us/download.

Then download or clone the [One Too (Self Balancing) Tree Repository](https://github.com/twtduck/GVSU-CIS641-One-Too-Self-Balancing-Tree). 

Because this application is under development, to use the Google API a key is required. This key can be obtained from the Google API Console.

Alternatively, you can contact me to get a key (note: I will need your Google account email address to add you as a test user for my key).

## Run Instructions

Navigate to the `src/OneTooCalendar` directory and run the following command:

    dotnet build

If the build fails, you are probably missing the api key file in `src/OneTooCalendar/OneTooCalendar/ApiKeys/`. Obtain key file (see above) and put it in that directory, named `credentials.json`.

After a successful build, in file explorer, navigate to `src/OneTooCalendar/OneTooCalendar/bin/Debug/net6.0-windows` and open the `OneTooCalendar.exe` file.

The first time the application is run (or if the access token has expired), you will be prompted to log in to your Google account. If a login is not detected in 60 seconds, the application will show an error and require a re-login.