REM http://msdn.microsoft.com/en-us/library/aa374928%28v=VS.85%29.aspx

rem "c:\program files\support tools\httpcfg" set urlacl /u http://+:8000/vre/vis/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"
rem "c:\program files\support tools\httpcfg" set urlacl /u http://+:8000/vre/buy/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"
rem "c:\program files\support tools\httpcfg" set urlacl /u http://+:8000/vre/sub/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"
rem "c:\program files\support tools\httpcfg" set urlacl /u http://+:8000/vre/sal/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"
rem "c:\program files\support tools\httpcfg" set urlacl /u http://+:8000/vre/dad/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"
httpcfg set urlacl /u http://+:8026/vre/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"
rem "c:\program files\support tools\httpcfg" set iplisten -i 127.0.0.1:8000

rem httpcfg set urlacl /u http://0.0.0.0:8000/vre/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"

"c:\program files\support tools\httpcfg" query urlacl
"c:\program files\support tools\httpcfg" query iplisten
PAUSE