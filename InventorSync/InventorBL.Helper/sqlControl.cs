
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Windows.Forms;
using InventorSync.InventorBL.Helper;
using System.Data;

namespace InventorSync.InventorBL.Helper
{
    public class sqlControl
    {
        private SqlConnection cn = new SqlConnection(); // (Properties.Settings.Default.ConnectionString)
        // Private mTransaction As SqlTransaction
        // Private mblnBeginTrans As Boolean
        // Private mblnCommitTrans As Boolean
        // Private mblnRollBackTrans As Boolean
        // Private mQueryString As String

        private string MyConnectionString;
        private bool mblnBeginTrans;
        private bool mblnCommitTrans;
        private bool mblnRollBackTrans;
        private string mQueryString;
        private bool mShowExceptionAutomatically;
        private bool mThrowMessageOnNoRecordsAffected;
        public string QueryString
        {
            get
            {
                return mQueryString;
            }
        }
        public SqlConnection connection
        {
            get
            {
                return cn;
            }
        }

        public bool ShowExceptionAutomatically
        {
            get
            {
                return mShowExceptionAutomatically;
            }
            set
            {
                mShowExceptionAutomatically = value;
            }
        }
        public bool ThrowMessageOnNoRecordsAffected
        {
            get
            {
                return mThrowMessageOnNoRecordsAffected;
            }
            set
            {
                mThrowMessageOnNoRecordsAffected = value;
            }
        }
        // Private cn As New SqlConnection("Data Source=LAPTOP\DCSSQL14;Initial Catalog=DCSStartup;User ID=sa;Password=NEWTECH007$")
        private SqlCommand cmd;

        private BindingSource _bsData;

        public BindingSource bsData
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _bsData;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_bsData != null)
                {
                }

                _bsData = value;
                if (_bsData != null)
                {
                }
            }
        }

        public SqlDataAdapter sqlDA;
        public DataTable sqlDT;

        public List<SqlParameter> Params = new List<SqlParameter>();

        public long RecordCount;
        public string Exception;

        public int CurrentRow = 0;

        public bool EndOfFile = false;
        public bool BeginningOfFile = false;

        public void Dispose()
        {
            try
            {

                // cn.Dispose()
                cmd.Dispose();
                if (bsData != null) bsData.Dispose();
                if (sqlDA != null) sqlDA.Dispose();
                if (sqlDT != null) sqlDT.Dispose();

                mblnBeginTrans = default(Boolean);
                mblnCommitTrans = default(Boolean);
                mblnRollBackTrans = default(Boolean);
                mQueryString = null;
                mShowExceptionAutomatically = default(Boolean);
                mThrowMessageOnNoRecordsAffected = default(Boolean);
                Params = null;
                RecordCount = 0;
                Exception = null;
                CurrentRow = 0;
                EndOfFile = default(Boolean);
                BeginningOfFile = default(Boolean);

                cn.Close();
                cn.Dispose();
            }
            catch (System.Exception ex)
            {
            }
        }

        // class initialising
        public sqlControl()
        {
            MyConnectionString = Properties.Settings.Default.ConnectionString;

            if (Global.mycn == null)
                Global.Setmycn(Properties.Settings.Default.ConnectionString);
            if (Global.mycn.ConnectionString == null)
                Global.Setmycn(Properties.Settings.Default.ConnectionString);
            if (Global.mycn.ConnectionString == "")
                Global.Setmycn(Properties.Settings.Default.ConnectionString);
            if (Global.mycn.ConnectionString != null)
            {
                if (Global.mycn.ConnectionString != "")
                {
                    if (Global.mycn.State != ConnectionState.Open)
                    {
                        try
                        {
                            if (Global.mycn.ConnectionString == null)
                                Global.mycn = new SqlConnection(MyConnectionString);
                            if (Global.mycn.ConnectionString == "")
                                Global.mycn = new SqlConnection(MyConnectionString);
                            if (Global.mycn.State == ConnectionState.Closed)
                                Global.mycn.Open();
                            else if (Global.mycn.State == ConnectionState.Broken)
                                Global.mycn.Open();
                            else if (Global.mycn.State == ConnectionState.Connecting)
                                return;
                            else if (Global.mycn.State == ConnectionState.Executing)
                                return;
                            else if (Global.mycn.State == ConnectionState.Fetching)
                                return;
                        }
                        catch (System.Exception ex)
                        {
                            try
                            {
                                throw ex;
                            }
                            catch (System.Exception ex1)
                            {
                                writeErrorLogText("OpenConnection", "SqlControl new method global.mycn open ");

                                if (ShowExceptionAutomatically == true)
                                    MessageBox.Show("Please check whether SQL Server " + Global.SqlServerName + " is started or not . Switching to secondary server " + Global.SqlServerName2 + " -> click ok to continue  " + ex1.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    //if (Global.mycn.State == ConnectionState.Open)
                    //    cn = Global.mycn;
                        
                    cn = Global.mycn;

                    mShowExceptionAutomatically = true;
                    OpenConnection(ref cn);
                }
            }
        }

        public string this[string FieldName]
        {
            get
            {
                return fields(FieldName);
            }
            set
            {
                Interaction.MsgBox("A value cannot be set to a field with this property.");
            }
        }



        public void bind(string Query, DataGridView dg)
        {
            try
            {
                // Using con As New SqlConnection(cn)
                using (SqlCommand cmd = new SqlCommand(Query, cn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            dg.DataSource = dt;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Exception = ex.Message;
                Interaction.MsgBox(Exception);
                try
                {
                    throw ex;
                }
                catch (System.Exception ex1)
                {
                    Interaction.MsgBox(ex.Message);
                }
            }
        }
        public DataSet readFromSp(string s, string[] param, string[] values)
        {
            DataSet drr = new DataSet();
            drr.Clear();
            try
            {
                SqlCommand Cmd11;
                // cn = New SqlConnection(Properties.Settings.Default.ConnectionString)
                // cn.Open()
                Cmd11 = new SqlCommand(s, cn);
                Cmd11.CommandType = CommandType.StoredProcedure;
                for (var i = 0; i <= param.Length - 1; i++)
                    Cmd11.Parameters.AddWithValue(param[i], values[i]);
                SqlDataAdapter da = new SqlDataAdapter(Cmd11);
                da.Fill(drr);
                Cmd11.Connection.Close();
            }
            // cn.Close()
            catch (System.Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
            return drr;
        }
        // ---------------------
        public bool BeginTrans
        {
            get
            {
                return mblnBeginTrans;
            }
            set
            {
                if (value == true)
                    //Transaction = cn.BeginTransaction();
                    Global.GSqlTransaction(ref cn, 1);
                else
                    RollbackTrans = true;
                mblnBeginTrans = value;
            }
        }

        public bool RollbackTrans
        {
            get
            {
                return mblnRollBackTrans;
            }
            set
            {
                if (value == true)
                {
                    if (mblnBeginTrans)
                    {
                        //Transaction.Rollback();
                        Global.GSqlTransaction(ref cn, 3);
                        mblnBeginTrans = false;
                    }
                }
                else
                {
                }
                mblnRollBackTrans = value;
            }
        }

        public bool CommitTrans
        {
            get
            {
                return mblnCommitTrans;
            }
            set
            {
                if (value == true)
                {
                    if (mblnBeginTrans)
                    {
                        //Transaction.Commit();
                        Global.GSqlTransaction(ref cn, 2);
                        mblnBeginTrans = false;
                    }
                }
                else
                    RollbackTrans = true;
                mblnCommitTrans = value;
            }
        }

        // Connection override (function overriding)
        public sqlControl(string ConnectionString, bool blnShowExceptionAutomatically = true)
        {
            MyConnectionString = ConnectionString;
            mShowExceptionAutomatically = blnShowExceptionAutomatically;
            ShowExceptionAutomatically = blnShowExceptionAutomatically;
            cn = new SqlConnection(ConnectionString);
            OpenConnection(ref cn);
        }

        private void OpenConnection(ref SqlConnection cn)
        {
            try
            {

                //MessageBox.Show(cn.ConnectionString);

                if (cn.ConnectionString == null)
                    cn = new SqlConnection(MyConnectionString);
                if (cn.ConnectionString == "")
                    cn = new SqlConnection(MyConnectionString);
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                else if (cn.State == ConnectionState.Broken)
                    cn.Open();
                else if (cn.State == ConnectionState.Connecting)
                    return;
                else if (cn.State == ConnectionState.Executing)
                    return;
                else if (cn.State == ConnectionState.Fetching)
                    return;
            }
            catch (System.Exception ex)
            {
                try
                {
                    throw ex;
                }
                catch (System.Exception ex1)
                {
                    writeErrorLogText("OpenConnection", "Flow moved to OpenConnection ");

                    if (ShowExceptionAutomatically == true)
                        MessageBox.Show("Please check whether SQL Server " + Global.SqlServerName + " is started or not . Switching to secondary server " + Global.SqlServerName2 + " -> click ok to continue  " + ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //MsgBox("Please check whether SQL Server " + DCSApp.SqlServerName + " is started or not . Switching to secondary server " + DCSApp.SqlServerName2 + " -> click ok to continue  " + ex.Message, MsgBoxStyle.SystemModal + MsgBoxStyle.Critical, "Open Connection");
                }
            }
        }
        public DataTable readbind(string s)
        {
            DataTable drr = new DataTable();
            SqlCommand Cmd11;
            // cn = New SqlConnection(Properties.Settings.Default.ConnectionString)
            // cn.Open()
            Cmd11 = new SqlCommand(s, cn);
            Cmd11.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(Cmd11);
            da.Fill(drr);
            Cmd11.Connection.Close();
            // cn.Close()
            return drr;
        }
        // Open query string
        public void Open(string Query)
        {
            mQueryString = Query;

            bsData = new BindingSource();

            if (Strings.Left(Strings.Trim(Query), 6).ToUpper() != "SELECT")
            {
            }

            // Reset query statistics
            RecordCount = 0;
            Exception = "";

            // Try
            if (cn.State != ConnectionState.Open)
                OpenConnection(ref cn);

            cmd = new SqlCommand();
            cmd.Connection = cn;

            bool blnBeginTransactionFromCurrentSub = false;
            if (Global.Transaction == null)
            {
                blnBeginTransactionFromCurrentSub = true;
                //Global.Transaction = cn.BeginTransaction();
                Global.GSqlTransaction(ref cn, 3);
            }

            cmd.Transaction = Global.Transaction;
            cmd.CommandText = Query;
            cmd.CommandTimeout = 0;
            // load parameters into database command
            Params.ForEach(p => cmd.Parameters.Add(p));

            // clear param list
            Exception = "";
            Params.Clear();
            
            if(bsData != null)
                bsData.Filter = null;
            
            // execute command and fill dataset
            sqlDT = new DataTable();

            try
            {
                try
                {
                    sqlDA = new SqlDataAdapter(cmd);
                }
                catch (System.Exception ex)
                {
                    Interaction.MsgBox(ex.Message);
                    Exception = ex.Message;

                    writeErrorLogText(Query, Exception);
                    if (mShowExceptionAutomatically)
                        ShowErrorMessage(true);
                }
                try
                {
                    if(bsData != null)
                        bsData.DataSource = null;
                }
                catch (System.Exception ex)
                {
                    Interaction.MsgBox(ex.Message);
                    Exception = ex.Message;

                    writeErrorLogText(Query, Exception);
                    if (mShowExceptionAutomatically)
                        ShowErrorMessage(true);
                }
                try
                {
                    bsData.DataSource = sqlDT;
                }
                catch (System.Exception ex)
                {
                    Interaction.MsgBox(ex.Message);
                    Exception = ex.Message;

                    writeErrorLogText(Query, Exception);
                    if (mShowExceptionAutomatically)
                        ShowErrorMessage(true);

                }
                try
                {
                    RecordCount = sqlDA.Fill(sqlDT);
                }
                catch (System.Exception ex)
                {
                    Interaction.MsgBox(ex.Message);
                    Exception = ex.Message;

                    writeErrorLogText(Query, Exception);
                    if (mShowExceptionAutomatically)
                        ShowErrorMessage(true);
                }

                if (sqlDT.Rows.Count > 0)
                    CurrentRow = 0;
                if (sqlDT.Rows.Count <= 0)
                    CurrentRow = -1;

                if (CheckBofEof(true))
                {
                    if (sqlDT.Rows.Count > 0)
                        CurrentRow = 0;
                }
                else if (sqlDT.Rows.Count <= 0)
                    CurrentRow = -1;

                if (RecordCount <= 0 & mThrowMessageOnNoRecordsAffected)
                {
                    writeErrorLogText(Query, "Unexpected error occurred. No records were effected with the statement.");
                    Exception e = new Exception("Unexpected error occurred. No records were effected with the statement.");
                    throw e;
                }
            }
            catch (System.Exception ex)
            {
                if (blnBeginTransactionFromCurrentSub == true)
                {
                    blnBeginTransactionFromCurrentSub = false;
                    //Transaction.Rollback();
                    Global.GSqlTransaction(ref cn, 3);
                }
                try
                {
                    throw ex;
                }
                catch (System.Exception ex1)
                {
                    Exception = ex1.Message;

                    writeErrorLogText(Query, Exception);
                    if (mShowExceptionAutomatically)
                        ShowErrorMessage(true);
                }
            }

            if (blnBeginTransactionFromCurrentSub == true)
            {
                blnBeginTransactionFromCurrentSub = false;
                //Transaction.Commit();
                Global.GSqlTransaction(ref cn, 2);
            }
        }

        public void writeErrorLogText(string Query, string ErrorName)
        {
            try
            {
                if (Directory.Exists(Application.StartupPath.ToString() + @"\ErrorLogs") == false)
                    Directory.CreateDirectory(Application.StartupPath.ToString() + @"\ErrorLogs");

                if (Directory.Exists(Application.StartupPath.ToString() + @"\ErrorLogs\" + Global.CompanyCode) == false)
                    Directory.CreateDirectory(Application.StartupPath.ToString() + @"\ErrorLogs\" + Global.CompanyCode);
            }
            catch 
            {
            }
            try
            {
                ClsFileOperation Myfile = new ClsFileOperation();
                string myfileNAme = Strings.Replace(Strings.Replace(Strings.Replace(Strings.Replace(Strings.Replace(DateTime.Now.Date.ToShortDateString(), ":", ""), @"\", ""), "-", ""), " ", ""), "/", "");
                myfileNAme = Application.StartupPath.ToString() + @"\ErrorLogs\" + Global.CompanyCode + @"\" + myfileNAme + ".txt";

                Myfile.FileOperation(myfileNAme, false, "Start" + DateTime.Now.ToShortTimeString() + Constants.vbCrLf + Query + Constants.vbCrLf + ErrorName + Constants.vbCrLf + Global.ComputerName + Constants.vbCrLf + "End", true);
            }
            catch
            {
            }
        }

        public void Close()
        {
            mQueryString = "";

            // Reset query statistics
            RecordCount = 0;
            Exception = "";

            cmd = new SqlCommand();

            //Global.Transaction = null;
            Global.GSqlTransaction(ref cn, 4);

            cmd.CommandText = "";
            cmd.Parameters.Clear();

            // clear param list
            Params.Clear();
            if (bsData != null)
                bsData.Filter = null;
            // execute command and fill dataset
            sqlDT = new DataTable();

            Exception = "";

            mblnBeginTrans = false;
            mblnCommitTrans = false;
            mblnRollBackTrans = false;
            bsData = new BindingSource();
            EndOfFile = true;
            BeginningOfFile = false;
            try
            {
                sqlDA = new SqlDataAdapter();
                bsData.DataSource = null;
                RecordCount = 0;

                CurrentRow = -1;

                cn.Close();
                cn.Dispose();
            }
            catch (System.Exception ex)
            {
                try
                {
                    throw ex;
                }
                catch (System.Exception ex1)
                {
                    Exception = ex1.Message;
                    if (mShowExceptionAutomatically)
                        ShowErrorMessage(true);
                }
            }
        }
        public void insertsp(string s, string[] param, string[] values)
        {
            try
            {
                SqlCommand Cmd11;
                cn = new SqlConnection(Properties.Settings.Default.ConnectionString);
                cn.Open();
                Cmd11 = new SqlCommand(s, cn);
                Cmd11.CommandType = CommandType.StoredProcedure;
                for (var i = 0; i <= param.Length - 1; i++)
                    Cmd11.Parameters.AddWithValue(param[i], values[i]);
                Cmd11.ExecuteNonQuery();
                Cmd11.Connection.Close();
            }
            // cn.Close()
            catch (System.Exception ex)
            {
                Interaction.MsgBox(ex.Message);
            }
        }

        // Execute query string other than open
        public void Execute(string Query)
        {
            // Reset query statistics
            RecordCount = 0;
            Exception = "";

            mQueryString = Query;

            // Try
            if (cn.State != ConnectionState.Open)
                OpenConnection(ref cn);

            cmd = new SqlCommand();
            cmd.CommandTimeout = 0;
            cmd.Connection = cn;
            cmd.Transaction = Global.Transaction;
            cmd.CommandText = Query;


            // load parameters into database command
            Params.ForEach(p => cmd.Parameters.Add(p));

            // clear param list
            Params.Clear();

            Exception = "";
            try
            {
                RecordCount = cmd.ExecuteNonQuery();

                if (RecordCount <= 0 & mThrowMessageOnNoRecordsAffected)
                {
                    writeErrorLogText(Query, "Unexpected error occurred. No records were effected with the statement.");
                    Exception e = new Exception("Unexpected error occurred. No records were effected with the statement.");
                    throw e;
                }
            }
            catch (System.Exception ex)
            {
                writeErrorLogText(Query, ex.Message);
                try
                {
                    throw ex;
                }
                catch (System.Exception ex1)
                {
                    Exception = ex1.Message;

                    if (mShowExceptionAutomatically)
                         ShowErrorMessage(true);
                }
            }

            CurrentRow = -1;
        }

        private void ExecuteQueryString(string Query)
        {
        }

        public void AddParam(string Name, object Value)
        {
            SqlParameter NewParam = new SqlParameter(Name, Value);
            Params.Add(NewParam);
        }

        private bool ShowErrorMessage(bool Report = false)
        {
            if (string.IsNullOrEmpty(Exception))
                return false;
            if (Report == true)
                Interaction.MsgBox(Exception, MsgBoxStyle.Critical, "Exception:");
            return true;
        }

        public int FieldCount()
        {
            try
            {

            int FieldValue;
            if (sqlDT == null)
            {
                Interaction.MsgBox("Please assign data to datarow.");
                return 0;
            }
            if (sqlDT.Rows.Count <= 0)
            {
                Interaction.MsgBox("Please assign data to datarow.");
                return 0;
            }

            if (CheckBofEof(true))
                FieldValue = sqlDT.Columns.Count;
            else
                FieldValue = 0;

            return FieldValue;
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public string FieldName(int Index)
        {
            try
            {
            string FieldValue;
            if (sqlDT == null)
            {
                Interaction.MsgBox("Please assign data to datarow.");
                return null;
            }

            if (Index < 0)
            {
                Interaction.MsgBox("Index should be greater than or equal to zero.");
                return null;
            }

            if (CheckBofEof(true))
                FieldValue = sqlDT.Columns[Index].Caption.ToString(); 
            else
                FieldValue = null;

            return FieldValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public string fields(int Index)
        {
            try
            {
            string FieldValue;
            if (sqlDT == null)
            {
                Interaction.MsgBox("Please assign data to datarow.");
                return null;
            }

            if (Index < 0)
            {
                Interaction.MsgBox("Index should be greater than or equal to zero.");
                return null;
            }

            if (CheckBofEof(true))
                FieldValue = sqlDT.Rows[CurrentRow][Index].ToString();
            else
                FieldValue = null;

            return FieldValue;
        }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public string fields(string ColumnName)
        {
            try
            { 
            string FieldValue;
            if (sqlDT == null)
            {
                Interaction.MsgBox("Please assign data to datarow.");
                return null;
            }

            if (ColumnName.ToString() == "")
            {
                Interaction.MsgBox("Please specify a columnname.");
                return null;
            }


            if (CheckBofEof(true))
                {
                    DataRow row = sqlDT.Rows[CurrentRow];
                    FieldValue = row[ColumnName].ToString();
                    //FieldValue = sqlDT.Rows[CurrentRow].Field<string>(ColumnName);
                }
                else
                FieldValue = Constants.vbNullString;

            return FieldValue;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

}

        public void MoveNext()
        {
            CurrentRow = CurrentRow + 1;
        }

        public void MovePrevious()
        {
            CurrentRow = CurrentRow - 1;
        }

        public void Move(int Index)
        {
            CurrentRow = Index;
        }

        public void MoveFirst()
        {
            CurrentRow = 0;
        }

        public void Find(string ColumnName, string Value, bool blnFirstOccurance = true, bool blnLastOccurance = false, int OccuranceIndex = -1)
        {
            try
            {

            int index = -1;
            int OccurCount = -1;

            if (CheckBofEof(true))
            {
                for (var i = 0; i <= sqlDT.Rows.Count - 1; i++)
                {
                    if (blnFirstOccurance)
                    {
                        if (Value.ToString() == sqlDT.Rows[i].Field<string>(ColumnName))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (blnLastOccurance)
                    {
                        if (Value.ToString() == sqlDT.Rows[i].Field<string>(ColumnName))
                            index = i;
                    }
                    if (OccurCount != OccuranceIndex)
                    {
                        if (Value.ToString() == sqlDT.Rows[i].Field<string>(ColumnName))
                        {
                            OccurCount = OccurCount + 1;
                            if (OccurCount == OccuranceIndex)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }
            }

            if (index == -1)
                CurrentRow = sqlDT.Rows.Count;
            else
                CurrentRow = index;

            CheckBofEof(true);
            return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Global.gblMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                CurrentRow = -1;
            }
        }

        public int Find(string strProperty, string Value)
        {
            try
            {
                int Index;
                Index = bsData.Find(strProperty, Value);

                return Index;
            }
            catch (System.Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return -1;
            }
        }

        public bool Filter(string strFilter)
        {
            try
            {
                bsData.Filter = strFilter;

                return true;
            }
            catch (System.Exception ex)
            {
                Interaction.MsgBox(ex.Message);
                return false;
            }
        }

        private bool CheckBofEof(bool blnSuppressMessage = false)
        {
            if (eof())
            {
                if (!blnSuppressMessage)
                    Interaction.MsgBox("EOF reached.");
                return false;
            }
            if (bof())
            {
                if (!blnSuppressMessage)
                    Interaction.MsgBox("BOF reached.");
                return false;
            }

            return true;
        }

        public bool eof()
        {
            try
            {
                if (sqlDT == null)
                    return true;
                if (CurrentRow >= Conversion.Val(sqlDT.Rows.Count.ToString()) | sqlDT.Rows.Count == 0)
                {
                    CurrentRow = sqlDT.Rows.Count;
                    BeginningOfFile = false;
                    EndOfFile = true;
                    return true;
                }

                return false;
            }
            catch 
            {
                return false;
            }
        }

        public bool bof()
        {
            try
            {
                if (sqlDT == null)
                    return true;
                if (CurrentRow < 0)
                {
                    CurrentRow = -1;
                    BeginningOfFile = true;
                    EndOfFile = false;
                    return true;
                }

                return false;
            }
            catch 
            {
                return false;
            }
        }
    }
}
