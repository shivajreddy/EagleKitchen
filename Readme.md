# A simple Revit API project as a boiler plate

What does this include
- References
	- RevitAPI.dll, RevitAPIUI.dll
	- PresentationCore, System.XAML, WindowsBase



How to duplicate this project.
1. copy past the root of this project folder
2. change the solution name
3. Change the project name
4. Right click on project, select properties, go to 'Application' tab
Change the 'Assembly Name' and 'Default Namespace'

5. Update references to the revitapi.dll and revitapiui.dll,
to the files in this copied folder

6. Update the .addin file
	- set name
	- set assembly, this is the location of compiled .dll file
	- fullclassname, this is the entry point of the application
	- client id
		- in VisualStudio press 'Tools' > 'Create GUID'
		- make a new guid using python: import uuid, uuid.uuid4()

7. Make sure to use the correct fullClassName for the commands


8. adding new icons ie., .png images for commands
	- you can add new .png to your project in any new directory also. but make sure to set the
	'Build Action' property to 'Embedded Resource' of the newly added .png file 
	- the 'ImageUtilities' class basically finds the embedded resources of given name, so this image
	can be in any directory in the given project, as long as it's an embedded resource.


9. Edit the 'Build Event', right click on the project, properties, 'Build Events' tab
	- `if not exist` make sure to use the new name of the project
	- `Xcopy` reaname the from .addin file, this says that this .addin file in the prject root is copied
	- `Xcopy` rename the to folder
	and in the 'Debug' tab of the project properties, make sure that proper revit version .exe is selected
