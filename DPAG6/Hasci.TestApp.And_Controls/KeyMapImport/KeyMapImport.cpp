// KeyMapImport.cpp : Defines the entry point for the application.
//
#define Cx70

#include "stdafx.h"
#include "log2file.h"

#ifdef Cx70
#include "C:\\Program Files (x86)\\Intermec\\Developer Library\\Include\\KBDTools.h"
#pragma comment(lib, "C:\\Program Files (x86)\\Intermec\\Developer Library\\Lib\\WCE600\\WM6\\Armv4i\\KBDToolsCPL.lib")
#endif

//#include "Keyboard Remap.h"
//#pragma comment (lib, "CPKBDMAP.lib")

	typedef int (*PFN_ITC_KeyMapImport)(LPTSTR);
	LPCTSTR fileList[] = { 
		L"\\Windows\\CK3kbdRemap.cpl", 
		L"\\Windows\\CN3kbdRemap.cpl", 
		L"\\Windows\\KbdRemapCN4.cpl", 
		L"\\Windows\\KbdRemapCN50.cpl", 
		L"\\Windows\\KbdRemapCS40.cpl",
		L"\\Windows\\KBDTools.cpl",
		NULL };

	
//==========================================================================================
// function implementations
//==========================================================================================
int existFile(TCHAR* filename)
{
	HANDLE hFile;

	hFile = CreateFile (filename,   // Open MYFILE.TXT
					  GENERIC_READ,           // Open for reading
					  FILE_SHARE_READ,        // Share for reading
					  NULL,                   // No security
					  OPEN_EXISTING,          // Existing file only
					  FILE_ATTRIBUTE_NORMAL,  // Normal file
					  NULL);                  // No template file

	if (hFile == INVALID_HANDLE_VALUE)
	{
		// Your error-handling code goes here.
		return FALSE;
	}
	else
	{
		CloseHandle(hFile);
		return TRUE;
	}
}

HINSTANCE loadLib(){
//	return LoadLibrary(L"\\Windows\\CK3kbdRemap.cpl");

	int i=0;
	//static HINSTANCE hLib;
	do{
		if(existFile((TCHAR *)fileList[i]))
		{
			//hLib=LoadLibrary(fileList[i]);
			return LoadLibrary(fileList[i]);;
		}
		i++;
	}while(fileList[i]!=NULL);
	return NULL;

}
TCHAR* getAppPath(){
	//get the app dir
	static TCHAR strPath[MAX_PATH];
	GetModuleFileName (NULL, strPath, MAX_PATH);
	//find the last backslash
	TCHAR* p;
	if ( p = wcsrchr ( strPath, '\\')) 
		*++p = _T('\0'); // zero-terminate at the last backslash
	//wsprintf(startPath, strPath);
	return strPath;
}

int WINAPI WinMain(	HINSTANCE hInstance,
					HINSTANCE hPrevInstance,
					LPTSTR    lpCmdLine,
					int       nCmdShow)
{
 	// TODO: Place code here.

	appendfile("\\KeyMapImport.log.txt");
	Add2Log(L"KeyMapImport v2.2 Cx70 started\n", true);
	Add2Log(L"=========================\n", false);
/*
	if(wcslen(lpCmdLine)==0){
		Add2Log(L"Missing argument to load!\n", false);
		Add2Log(L"=========================\n", false);
		return -1;
	}
*/
	Add2Log(L"cmdLine='", false);
	Add2Log(lpCmdLine, false);
	Add2Log("'\n",false);
#ifdef Cx70
	TCHAR* xmlFile = new TCHAR[MAX_PATH]; //L"\\keymap.xml";
	if(wcslen(lpCmdLine)!=0){
		wsprintf(xmlFile, L"%s", lpCmdLine);
	}
	else{
		TCHAR strPath[MAX_PATH];
		wsprintf(strPath, L"%s", getAppPath());
		wsprintf(xmlFile, L"%skeymap.xml", strPath);
	}


	if(!existFile(xmlFile)){
		Add2Log(L"Missing xml file: '",false);
		Add2Log(xmlFile, false);
		Add2Log(L"'\nExecution terminated\n", false);
		Add2Log(L"=========================\n", false);
		return -4;
	}

	if (SetKeyboardNotify(FALSE)==0)
		Add2Log(L"ITC_KeyMapImport SetKeyboardNotify false OK\n", false);
	else
		Add2Log(L"ITC_KeyMapImport SetKeyboardNotify false Failed\n", false);

	int ret = ImportKeys(xmlFile, FALSE);

	if(ret==0){
		Add2Log(L"ITC_KeyMapImport ImportKeys OK\n", false);

		if (SetKeyboardNotify(TRUE)==0)
			Add2Log(L"ITC_KeyMapImport SetKeyboardNotify TRUE OK\n", false);
		else
			Add2Log(L"ITC_KeyMapImport SetKeyboardNotify TRUE Failed\n", false);
	}
	else
	{
		Add2Log(L"ITC_KeyMapImport ImportKeys() failed.\nExecution terminated\n", false);
	}
#else
	HINSTANCE hLib = NULL;

	hLib = loadLib(); //LoadLibrary(L"CPKBDMap.cpl");
	if (hLib == NULL)
	{
		Add2Log(L"Could not load keybd remapper cpl.\nExecution terminated\n", false);
		return -2;
	}

	PFN_ITC_KeyMapImport ITC_KeyMapImport;

	ITC_KeyMapImport = (PFN_ITC_KeyMapImport)GetProcAddress(hLib, _T("ITC_KeyMapImport"));
	if (ITC_KeyMapImport == NULL)
	{
		ITC_KeyMapImport = (PFN_ITC_KeyMapImport)GetProcAddress(hLib, _T("ITC_ImportKeyMap")); //special for CN50
		if (ITC_KeyMapImport == NULL)
		{
			ITC_KeyMapImport = (PFN_ITC_KeyMapImport)GetProcAddress(hLib, _T("ImportKeys"));	//special for coz
			if (ITC_KeyMapImport == NULL)
			{
					//all imports failed
				Add2Log(L"Could not load proc ITC_KeyMapImport.\nExecution terminated\n", false);
				Add2Log(L"=========================\n", false);
				FreeLibrary(hLib);
				return -3;
			}
			else
				Add2Log(L"Using 'ImportKeys()'\n", false);
		}
		else
			Add2Log(L"Using 'ITC_ImportKeyMap()'\n", false);
	}
	else
		Add2Log(L"Using 'ITC_KeyMapImport()'\n", false);

	TCHAR* xmlFile = new TCHAR[MAX_PATH]; //L"\\keymap.xml";
	if(wcslen(lpCmdLine)!=0){
		wsprintf(xmlFile, L"%s", lpCmdLine);
	}
	else{
		TCHAR strPath[MAX_PATH];
		wsprintf(strPath, L"%s", getAppPath());
		wsprintf(xmlFile, L"%skeymap.xml", strPath);
	}


	if(!existFile(xmlFile)){
		Add2Log(L"Missing xml file: '",false);
		Add2Log(xmlFile, false);
		Add2Log(L"'\nExecution terminated\n", false);
		Add2Log(L"=========================\n", false);
		FreeLibrary(hLib);
		return -4;
	}

	int ret = ITC_KeyMapImport(xmlFile);
	if (ret != 0)
	{
		Add2Log(L"ITC_KeyMapImport failed.\nExecution terminated\n", false);
		Add2Log(L"=========================\n", false);
		FreeLibrary(hLib);
		return ret;
	}
	else
	{
		Add2Log(L"ITC_KeyMapImport(\\keymap.xml) SUCCESS.\nExecution ended normally.\n", false);
		Add2Log(L"=========================\n", false);
		FreeLibrary(hLib);
		return ret;
	}
	FreeLibrary(hLib);
#endif
	return ret;
}

