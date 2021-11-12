## Overview
SQLite and .Net (Core) based application aimed to support cross-platform audio streaming / listening experience.
Current versions support :
WPF .Net 4.0 builds for windows;
WPF .Net Core 3.1 build for windows;
.NetStandard Xamarin Forms several component's support (in development);

## Usage - output:
Compile the application using visual studio for your target platform and run MediaStreamer.WPF.NetCore3.1 / FirstFMCourse.exe from build directory.

## Current Tasks (TODO's):
Integrate the XamarinMediaManager into the Windows (WPF) Application for both Net40 (EF 6) and NetCore3.1 (EFCore 5);

RAMControl:
	Connect WPFComponents with RAMControl (Exclude Program, Session, SessionInformation, FirstFMPage)
	Move CompositionStorage to RAMControl
	
XamarinForms:
	Implement the "Player functionality" on "Browse" page
	