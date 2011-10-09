Readme for VR Estate Server installation
----------------------------------------



I. Prerequisites
================


1. OS (32 or 64 bit)
	- Windows XP SP3
	- Windows 7
	- Windows 2008 R2 SP1

2. Windows XP SP2 Support Tools (Windows XP only)

	Downloadable from here:
	http://www.microsoft.com/downloads/en/details.aspx?familyid=49AE8576-9BB9-4126-9761-BA8011FABF38

	You may need to restart machine for tools to install in full.

3. Microsoft .NET Framework 4.0 Full Profile

	Downloadable from here (internet-based installer):
	http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

	or from here (standalone installer):
	http://www.microsoft.com/downloads/en/details.aspx?familyid=0a391abd-25c1-4fc0-919f-b21f31ab88b7

4. Microsoft Windows Installer (MSI) 4.5 (requlred for database)

	Downloadable from here:
	http://www.microsoft.com/downloads/en/details.aspx?FamilyId=5A58B56F-60B6-4412-95B9-54D056D6F9F4
	Windows6.0-KB942288-v2-x86.msu	- 32-bit
	Windows6.0-KB942288-v2-x64.msu	- 64-bit

5. Microsoft SQL Server 2005 or newer; any edition

	Downloadable from here:
	http://www.microsoft.com/downloads/en/details.aspx?FamilyID=01af61e6-2f63-4291-bcad-fd500f6027ff
	SQLEXPR32_x86_ENU.exe	- 32-bit
	SQLEXPR_x64_ENU.exe	- 64-bit



II. Service installation
========================

Open install.bat; make sure INSTPATH variable points to desired install location.

Run install.bat



III. Database setup
===================

Navigate to SQL subfolder.

Open VR-Setup.sql and verify setings in header.

Run sqle-install.bat to setup database on local SQL Express server;
otherwise execute the VR-Setup.sql script on desired server.







20110602

