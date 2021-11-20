## Overview
SQLite and .Net (Core) based application aimed to support cross-platform audio streaming / listening experience.
Current versions support :
WPF .Net 4.0 builds for Windows;
WPF .Net Core 3.1 build for Windows;
.NetStandard Xamarin Forms several component's support (in development);

## Usage - output:
Compile the application using visual studio for your target platform and run MediaStreamer.WPF.NetCore3.1 / FirstFMCourse.exe from build directory.

## Current Tasks (TODO's):

Overall //TODO: 
	Refactor into separate Methods those that consist more than of 16-21 lines;
	Delete unused code (if it's obsolete / not working);
	Connect WebScrobbler from Last.FM and its WEB APIs;
	Implement "Return to previous state (Composition / Player Position)" when application's closed and re-opened.
	
WPF.Components:
	CompositionsPage: Fix "Rename to Standard" check menu item button -> enable renaming the file to match pattern "Artist – Title (Year if exists)".
	
WPF.Net40 / WPFNetCore3.1 / DataAccess.Net40 / DataAccessNetCore3.1
	Merge into single project WPF's with WPF Targeted net40 and netcoreapp3.1
	
RAMControl (WPF/Components):
	Integrate the XamarinMediaManager into the Windows (WPF) Application for both Net40 (EF 6) and NetCore3.1 (EFCore 5);
	Connect WPFComponents with RAMControl (Exclude Program, Session, SessionInformation, FirstFMPage from WPF, add their references to RAMControl);
	Move CompositionStorage to RAMControl.
	
XamarinForms:
	(v) Implement the "Player functionality" on "Browse" page:
		Stop, Pause, Next, Previous;
	Implement slider bar to scroll playing compositions;
	Implement "Queue" from XamarinMediaManager project (allow queueing compositions);
	Implement "Video" page in Xamarin Forms;
	Implement "ListenedCompositions / UserCompositions" Model, ViewModel, View 
	(Load ListenedCompositions from DataBase, register the user's composition into database when composition is playing, see the WPF project for example code).
	
IO:
	Implement "Play several songs cross-platformely" with XamarinMediaManager (for both .Net 4.0 and .Net Core 3.1).


