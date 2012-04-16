#include <windows.h>
#include <AtlBase.h>
#include <AtlConv.h>
#include <deque>
#include "sqlite3.h"
#include "sqlitedll.h"

WCHAR* AllocAndCopy(const WCHAR* szString)
{
    size_t nLen = wcslen(szString) + 1;
    WCHAR* szRet = (WCHAR*) LocalAlloc(LMEM_FIXED, (nLen) * sizeof(WCHAR));
    if (szRet)
    {
        wcscpy_s(szRet, nLen, szString);
    }

    return szRet;
}

int StoreCallback(void * lpVoid, int argc, WCHAR **argv, WCHAR **pszColName)
{
    RowsRet * pRows = (RowsRet*) lpVoid;

    if (pRows->Rows.empty())
    {
        pRows->nCount ++;

        pRows->nCols = argc;
        deque<wstring> row;
        for (int i = 0; i < argc; i ++)
        {
            row.push_back(pszColName[i]);
        }
        pRows->Rows.push_back(row);;
    }

    if (argv)
    {
        pRows->nCount ++;

        deque<wstring> row;
        for (int i = 0; i < argc; i ++)
        {
            if (argv[i])
            {
                row.push_back(argv[i]);
            }
            else
            {
                row.push_back(L"");
            }
        }
        pRows->Rows.push_back(row);
    }

    return 0;
}

extern "C" WCHAR* __stdcall GetSQLiteVersion()
{
#ifdef WIN64BIT
    return AllocAndCopy(_T(SQLITE_VERSION) _T(" (64 bit)"));
#else
    return AllocAndCopy(_T(SQLITE_VERSION) _T(" (32 bit)"));
#endif
}

extern "C" HANDLE __stdcall OpenDatabase(WCHAR * szDatabase)
{
    DbHandle * dbh = new DbHandle();
    dbh->szLastError = NULL;

    wcscpy_s(dbh->szDBName, szDatabase);

    sqlite3_open16(dbh->szDBName, &(dbh->db));

    return (HANDLE)dbh;
}

extern "C" void __stdcall CloseDatabase(HANDLE hDB)
{
    DbHandle * dbh = (DbHandle*) hDB;
    sqlite3_close(dbh->db);
    dbh->db = NULL;
    delete dbh->szLastError;
    delete dbh;
}

int sqlite3_exec_utf16(sqlite3 *db,
                       const WCHAR *zSql,
                       sqlite3_callback_utf16 xCallback,
                       void *pArg,
                       BindHandle * bind)
{
    int rc = SQLITE_OK;
    const WCHAR *zLeftover;
    sqlite3_stmt *pStmt = 0;
    WCHAR **azCols = 0;

    int nRetry = 0;
    int nCallback;

    if (zSql==0)
    {
        return SQLITE_OK;
    }

    while((rc==SQLITE_OK || (rc==SQLITE_SCHEMA && (++nRetry)<2)) && zSql[0])
    {
        int nCol;
        WCHAR **azVals = 0;

        pStmt = 0;
        rc = sqlite3_prepare16(db, zSql, -1, &pStmt, (const void **) &zLeftover);
        if (rc!=SQLITE_OK)
        {
            continue;
        }

        if(!pStmt)
        {
            zSql = zLeftover;
            continue;
        }

        if (bind)
        {
            for (int i = 0; i < (int)bind->BindObjects.size(); i ++)
            {
                switch (bind->BindObjects[i].Type)
                {
                case typeString:
                    sqlite3_bind_text16(pStmt, i + 1, (void*) bind->BindObjects[i].String, -1, SQLITE_TRANSIENT);
                    break;
                default:
                    sqlite3_bind_text16(pStmt, i + 1, (void*) L"(unknown type)", -1, SQLITE_TRANSIENT);
                    break;
                }
            }
        }

        nCallback = 0;
        nCol = sqlite3_column_count(pStmt);

        while (true)
        {
            int i;
            rc = sqlite3_step(pStmt);

            if (xCallback && (SQLITE_ROW==rc || (SQLITE_DONE==rc && !nCallback )))
            {
                if (0==nCallback)
                {
                    if (azCols==0)
                    {
                        azCols = (WCHAR**) sqlite3_malloc(2*nCol*sizeof(const WCHAR*) + 1);
                        if (azCols==0)
                        {
                            goto exec_out;
                        }
                    }
                    for (i=0; i<nCol; i++)
                    {
                        azCols[i] = (WCHAR *)sqlite3_column_name16(pStmt, i);
                    }
                    nCallback++;
                }
                if (rc==SQLITE_ROW)
                {
                    azVals = &azCols[nCol];
                    for (i=0; i<nCol; i++)
                    {
                        azVals[i] = (WCHAR *)sqlite3_column_text16(pStmt, i);
                    }
                }
                if (xCallback(pArg, nCol, azVals, azCols))
                {
                    rc = SQLITE_ABORT;
                    goto exec_out;
                }
            }

            if (rc!=SQLITE_ROW)
            {
                rc = sqlite3_finalize(pStmt);
                pStmt = 0;
                if (rc!=SQLITE_SCHEMA)
                {
                    nRetry = 0;
                    zSql = zLeftover;
                    while (isspace((unsigned char)zSql[0])) 
                    {
                        zSql++;
                    }
                }
                break;
            }
        }

        sqlite3_free(azCols);
        azCols = 0;
    }

exec_out:
    if (pStmt)
    {
        sqlite3_finalize(pStmt);
    }
    if (azCols)
    {
        sqlite3_free(azCols);
    }

    return rc;
}

extern "C" HANDLE __stdcall ExecuteCommand(HANDLE hDB, WCHAR * szCommand, HANDLE hBind)
{
    RowsRet * pRows = new RowsRet();
    pRows->nCount = 0;

    BindHandle * bind = (BindHandle*) hBind;
    DbHandle * dbh = (DbHandle*) hDB;

    char * szErrMsg = NULL;
    int rc = sqlite3_exec_utf16(dbh->db, 
        szCommand, 
        StoreCallback, 
        pRows,
        bind);

    if (rc != SQLITE_OK)
    {
        if (dbh->szLastError)
        {
            delete dbh->szLastError;
        }
        WCHAR * szErr = (WCHAR*) sqlite3_errmsg16(dbh->db);
        size_t nLen = wcslen(szErr) + 1;
        dbh->szLastError = new WCHAR[nLen];
        wcscpy_s(dbh->szLastError, nLen, szErr);

        delete pRows;
        pRows = NULL;
    }

    return (HANDLE)pRows;
}

extern "C" void __stdcall GetTableSize(HANDLE hDB, HANDLE hRet, int * pnRows, int * pnCols)
{
    RowsRet * pRows = (RowsRet*) hRet;

    (*pnRows) = pRows->nCount;
    (*pnCols) = pRows->nCols;
}

extern "C" WCHAR* __stdcall GetDBLastError(HANDLE hDB)
{
    DbHandle* dbh = (DbHandle*) hDB;

    return AllocAndCopy(dbh->szLastError);
}

extern "C" WCHAR* __stdcall GetString(HANDLE hDB, HANDLE hRet, int nRow, int nCol)
{
    RowsRet * pRows = (RowsRet*) hRet;

    wstring sRet = pRows->Rows[nRow][nCol];

    return AllocAndCopy(sRet.c_str());
}

extern "C" void __stdcall CloseReturn(HANDLE hDB, HANDLE hRet)
{
    RowsRet * pRows = (RowsRet*) hRet;

    delete pRows;
}

extern "C" HANDLE __stdcall CreateBindObject(HANDLE hDB)
{
    BindHandle * ret = new BindHandle();

    return (HANDLE) ret;
}

extern "C" void __stdcall CloseBindObject(HANDLE hDB, HANDLE hBind)
{
    BindHandle * bind = (BindHandle*) hBind;

    for (int i = 0; i < (int)bind->BindObjects.size(); i ++)
    {
        bind->BindObjects[i].CleanUp();
    }

    delete bind;
}

extern "C" void __stdcall AddStringToBind(HANDLE hDB, HANDLE hBind, WCHAR * szData)
{
    BindHandle * bind = (BindHandle*) hBind;

    BindObject obj;

    obj.Type = typeString;
    obj.String = szData;

    bind->BindObjects.push_back(obj);    
}

extern "C" __int64 __stdcall GetLastRowID(HANDLE hDB)
{
    DbHandle * dbh = (DbHandle*) hDB;
    return sqlite3_last_insert_rowid(dbh->db);
}

extern "C" int __stdcall GetChanges(HANDLE hDB)
{
    DbHandle * dbh = (DbHandle*) hDB;
    return sqlite3_changes(dbh->db);
}

extern "C" void __stdcall GetAllExports(int cookie, Delegates * results)
{
    if (cookie == 0x0ef1590f)
    {
        results->AddStringToBind = (FARPROC) AddStringToBind;
        results->CloseBindObject = (FARPROC) CloseBindObject;
        results->CloseDatabase = (FARPROC) CloseDatabase;
        results->CloseReturn = (FARPROC) CloseReturn;
        results->CreateBindObject = (FARPROC) CreateBindObject;
        results->ExecuteCommand = (FARPROC) ExecuteCommand;
        results->GetChanges = (FARPROC) GetChanges;
        results->GetDBLastError = (FARPROC) GetDBLastError;
        results->GetLastRowID = (FARPROC) GetLastRowID;
        results->GetSQLiteVersion = (FARPROC) GetSQLiteVersion;
        results->GetString = (FARPROC) GetString;
        results->GetTableSize = (FARPROC) GetTableSize;
        results->OpenDatabase = (FARPROC) OpenDatabase;
    }
}
