using namespace std;

struct DbHandle
{
    sqlite3* db;
    WCHAR   szDBName[MAX_PATH];
    WCHAR*  szLastError;
};

struct RowsRet
{
    int nCount;
    int nCols;
    deque<deque<wstring>> Rows;
};

enum ObjectTypes
{
    typeString = 1,
};

class BindObject
{
public:
    ObjectTypes Type;
    WCHAR* String;

    BindObject()
    {
        String = NULL;
    }

    void CleanUp()
    {
        if (String)
        {
            LocalFree(String);
        }
    }
};

struct BindHandle
{
    deque<BindObject> BindObjects;
};

struct Delegates
{
    FARPROC AddStringToBind;
    FARPROC CloseBindObject;
    FARPROC CloseDatabase;
    FARPROC CloseReturn;
    FARPROC CreateBindObject;
    FARPROC ExecuteCommand;
    FARPROC GetChanges;
    FARPROC GetDBLastError;
    FARPROC GetLastRowID;
    FARPROC GetSQLiteVersion;
    FARPROC GetString;
    FARPROC GetTableSize;
    FARPROC OpenDatabase;
};

typedef int (*sqlite3_callback_utf16)(void*,int,WCHAR**, WCHAR**);

WCHAR* AllocAndCopy(const WCHAR* szString);
int StoreCallback(void * lpVoid, int argc, WCHAR **argv, WCHAR **pszColName);
extern "C" WCHAR* __stdcall GetSQLiteVersion();
extern "C" HANDLE __stdcall OpenDatabase(WCHAR * szDatabase);
extern "C" void __stdcall CloseDatabase(HANDLE hDB);
int sqlite3_exec_utf16(sqlite3 *db, const WCHAR *zSql, sqlite3_callback_utf16 xCallback, void *pArg, BindHandle * bind);
extern "C" HANDLE __stdcall ExecuteCommand(HANDLE hDB, WCHAR * szCommand, HANDLE hBind);
extern "C" void __stdcall GetTableSize(HANDLE hDB, HANDLE hRet, int * pnRows, int * pnCols);
extern "C" WCHAR* __stdcall GetDBLastError(HANDLE hDB);
extern "C" WCHAR* __stdcall GetString(HANDLE hDB, HANDLE hRet, int nRow, int nCol);
extern "C" void __stdcall CloseReturn(HANDLE hDB, HANDLE hRet);
extern "C" HANDLE __stdcall CreateBindObject(HANDLE hDB);
extern "C" void __stdcall CloseBindObject(HANDLE hDB, HANDLE hBind);
extern "C" void __stdcall AddStringToBind(HANDLE hDB, HANDLE hBind, WCHAR * szData);
extern "C" __int64 __stdcall GetLastRowID(HANDLE hDB);
extern "C" int __stdcall GetChanges(HANDLE hDB);
extern "C" void __stdcall GetAllExports(int cookie, Delegates * results);
