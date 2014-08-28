Job Scheduler
=============
This sample demonstrates the new JobScheduler API. The JobScheduler API is part of Android-L's 'Project Volta' which provides new APIs to increase battery life.

JobScheduler allows applications to request non-user facing tasks to be run by the Android system in such a way as to maximize battery life. 

This sample doesn't do any actual 'work', but demonstrates how to use the API.

Instructions
------------
* Select any parameters from the main screen to restrict when the job can run.
* Set a small deadline time. Android will automatically run the job after the deadline has passed.
* Hit 'Schedule Job'
* The job will begin running no latter than the deadline. When the job begins, onStartTask will light up green.
* When the job terminates, onStopTask will light up red.

Build Requirements
------------------
Download the latest version of Xamarin Studio. Open JobScheduler.sln in Xamarin Studio and run the project. This project requires the API level 21 (L) SDK platform.

Author
------
Copyright (c) 2005-2008, The Android Open Source Project  
Ported to Xamarin.Android by Ben O'Halloran
