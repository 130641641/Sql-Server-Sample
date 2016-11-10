using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;

namespace MyOnLineExam.DataAccessLayer
{
  // ﾊｾﾝｿ篆ﾓｿﾚﾀ・
	public class DataBase
	{
		//ﾋｽﾓﾐｱ菽ｿ｣ｬﾊｾﾝｿ簔ｬｽﾓ
		protected SqlConnection Connection;
        protected string ConnectionString;

		//ｹｹﾔ・ｯﾊ
		public DataBase()
		{
            ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
		
		//ｱ｣ｻ､ｷｽｷｨ｣ｬｴｪﾊｾﾝｿ簔ｬｽﾓ
		private void Open()
		{
		  //ﾅﾐｶﾏﾊｾﾝｿ簔ｬｽﾓﾊﾇｷ贇ﾚ
			if (Connection == null)
			{
			  //ｲｻｴ贇ﾚ｣ｬﾐﾂｽｨｲ｢ｴｪ
				Connection = new SqlConnection(ConnectionString);
				Connection.Open();
			}
			else
			{
			  //ｴ贇ﾚ｣ｬﾅﾐｶﾏﾊﾇｷｦﾓﾚｹﾘｱﾕﾗｴﾌｬ
			  if (Connection.State.Equals(ConnectionState.Closed))
				  Connection.Open();    //ﾁｬｽﾓｴｦﾓﾚｹﾘｱﾕﾗｴﾌｬ｣ｬﾖﾘﾐﾂｴｪ
			}
		}

		//ｹｫﾓﾐｷｽｷｨ｣ｬｹﾘｱﾕﾊｾﾝｿ簔ｬｽﾓ
		public void Close() 
		{
			if (Connection.State.Equals(ConnectionState.Open))
			{
				Connection.Close();     //ﾁｬｽﾓｴｦﾓﾚｴｪﾗｴﾌｬ｣ｬｹﾘｱﾕﾁｬｽﾓ
			}
		}

        /// <summary>
		/// ﾎｹｺｯﾊ｣ｬﾊﾍｷﾅｷﾇﾍﾐｹﾜﾗﾊﾔｴ
		/// </summary>
		~DataBase()
		{
			try
			{
				if (Connection != null)
					Connection.Close();
			}
			catch{}
			try
			{
				Dispose();
			}
			catch{}
		}

		//ｹｫﾓﾐｷｽｷｨ｣ｬﾊﾍｷﾅﾗﾊﾔｴ
		public void Dispose()
		{
			if (Connection != null)		// ﾈｷｱ｣ﾁｬｽﾓｱｻｹﾘｱﾕ
			{
				Connection.Dispose();
				Connection = null;
			}
		}		

		//ｹｫﾓﾐｷｽｷｨ｣ｬｸﾝSqlﾓ・茱ｬｷｵｻﾘﾊﾇｷ鰉ｯｵｽｼﾇﾂｼ
		public bool GetRecord(string XSqlString)
		{
            Open();
            SqlDataAdapter adapter = new SqlDataAdapter(XSqlString, Connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            Close();

            if (dataset.Tables[0].Rows.Count > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		//ｹｫﾓﾐｷｽｷｨ｣ｬｷｵｻﾘSqlﾓ・莉ﾃｵﾄﾊｾﾝﾖｵ
		//SqlStringｵﾄｸｽ｣ｺselect count(*) from XXX where ...
		//                 select max(XXX) from YYY where ...
		public int GetRecordCount(string XSqlString)
		{
            string SCount;

			Open();
			SqlCommand Cmd = new SqlCommand(XSqlString,Connection);
            SCount = Cmd.ExecuteScalar().ToString().Trim();
            if (SCount=="")
            SCount="0";
			Close();
			return Convert.ToInt32(SCount);
		}

        //ｹｫﾓﾐｷｽｷｨ｣ｬｷｵｻﾘSqlﾓ・莉ﾃｵﾄﾊｾﾝ
        //SqlStringｵﾄｸｽ｣ｺselect string from XXX where ...
        public string GetString(string SqlString)
        {
            string SCount;

            Open();
            SqlCommand Cmd = new SqlCommand(SqlString, Connection);
            SCount = Cmd.ExecuteScalar().ToString().Trim();
            
            Close();
            return SCount;
        }	

		//ｹｫﾓﾐｷｽｷｨ｣ｬｸﾝXWhereｸ・ﾂﾊｾﾝｱ偀TableNameﾖﾐｵﾄﾄｳﾐｩｼﾍﾂｼ
		//XTableName--ｱ朎・
		//XHT--ｹﾏ｣ｱ惞ｬｼ・ｪﾗﾖｶﾎﾃ訒ｬﾖｵﾎｪﾗﾖｶﾎﾖｵ		
		public DataSet AdvancedSearch(string XTableName, Hashtable XHT)
		{
			int Count = 0;

			string Fields = "";
			foreach(DictionaryEntry Item in XHT)
			{
				if (Count != 0)
				{
					Fields += " and ";
				}
				Fields += Item.Key.ToString();
				Fields += " like '%";
				Fields += Item.Value.ToString();
                Fields += "%'";
				Count++;
			}
			Fields += " ";

			string SqlString = "select * from " + XTableName + " where " + Fields;
            Open();
            SqlDataAdapter Adapter = new SqlDataAdapter(SqlString, Connection);
            DataSet Ds = new DataSet();
            Adapter.Fill(Ds);
            Close();
            return Ds;
			
		}		

        //ﾋｽﾓﾐｷｽｷｨ｣ｬｻﾃﾒｻｸﾃﾀｴｵﾃｴ豢｢ｹｳﾌｵﾄSqlCommand
        //ﾊ菠・ｺ
        //      ProcName - ｴ豢｢ｹｳﾌﾃ・
        //      Params   - ﾓﾃﾀｴｵﾃｴ豢｢ｹｳﾌｵﾄｲﾎﾊｱ・
        private SqlCommand CreateCommand(string ProcName, SqlParameter[] Prams) 
        {
          Open();
          SqlCommand Cmd = new SqlCommand(ProcName, Connection);
          Cmd.CommandType = CommandType.StoredProcedure;

          if (Prams != null) 
          {
            foreach (SqlParameter Parameter in Prams)
              Cmd.Parameters.Add(Parameter);
          }

          return Cmd;
        }

        //ｹｫﾓﾐｷｽｷｨ｣ｬﾊｵﾀｻｯﾒｻｸﾃﾓﾚｵﾃｴ豢｢ｹｳﾌｵﾄｲﾎﾊ
        //ﾊ菠・ｺ
        //      ParamName - ｲﾎﾊﾃ﨤ﾆ
        //      DbType		- ｲﾎﾊﾀ獎ﾍ
        //      Size			- ｲﾎﾊｴ｡
        //			Direction - ｴｫｵﾝｷｽﾏ・
        //			Value			- ﾖｵ
        public SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value) 
        {
          SqlParameter Param;

          if(Size > 0)
            Param = new SqlParameter(ParamName, DbType, Size);
          else Param = new SqlParameter(ParamName, DbType);

          Param.Direction = Direction;

          if (Value != null)
            Param.Value = Value;

          return Param;
        }

		//ｹｫﾓﾐｷｽｷｨ｣ｬﾊｵﾀｻｯﾒｻｸﾃﾓﾚｵﾃｴ豢｢ｹｳﾌｵﾄﾊ菠・ﾎﾊ
		//ﾊ菠・ｺ
		//      ParamName - ｲﾎﾊﾃ﨤ﾆ
		//      DbType		- ｲﾎﾊﾀ獎ﾍ
		//      Size			- ｲﾎﾊｴ｡
		//			Value			- ﾖｵ
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value) 
        {
          return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }		

        //ｹｫﾓﾐｷｽｷｨ｣ｬｵﾃｴ豢｢ｹｳﾌ(ｲｻｴﾎﾊ)
		    //ﾊ菠・ｺ
		    //			ProcNameｴ豢｢ｹｳﾌﾃ・
        //ﾊ莎ｺ
		    //      ｶﾔUpdate｡｢Insert｡｢Deleteｲﾙﾗｵｻﾘﾓｰﾏ・ｽｵﾄﾐﾐﾊ｣ｬﾆ萢鉎鯀ｪ-1
        public int RunProc(string ProcName) 
        {
		    int Count = -1;
            SqlCommand Cmd = CreateCommand(ProcName, null);
            Count = Cmd.ExecuteNonQuery();
            Close();
			return Count;
        }

        //ｹｫﾓﾐｷｽｷｨ｣ｬｵﾃｴ豢｢ｹｳﾌ(ｴﾎﾊ)
        //ﾊ菠・ｺ
        //      ProcName - ｴ豢｢ｹｳﾌﾃ・
        //      Params   - ﾓﾃﾀｴｵﾃｴ豢｢ｹｳﾌｵﾄｲﾎﾊｱ・
        //ﾊ莎ｺ
        //      ｶﾔUpdate｡｢Insert｡｢Deleteｲﾙﾗｵｻﾘﾓｰﾏ・ｽｵﾄﾐﾐﾊ｣ｬﾆ萢鉎鯀ｪ-1
        public int RunProc(string ProcName, SqlParameter[] Params) 
        {
          int Count = -1;
          SqlCommand Cmd = CreateCommand(ProcName, Params);
          Count = Cmd.ExecuteNonQuery();
          Close();
          return Count;
        }

        //ｹｫﾓﾐｷｽｷｨ｣ｬｵﾃｴ豢｢ｹｳﾌ(ｲｻｴﾎﾊ)
        //ﾊ菠・ｺ
        //			ProcNameｴ豢｢ｹｳﾌﾃ・
		    //ﾊ莎ｺ
        //			ｽｫﾖｴﾐﾐｽ盪鋐ﾔSqlDataReaderｷｵｻﾘ
		    //ﾗ｢ﾒ筌ｺﾊｹﾓﾃｺ箏ﾃSqlDataReader.Close()ｷｽｷｨ
        public SqlDataReader RunProcGetReader(string ProcName) 
        {
          SqlCommand Cmd = CreateCommand(ProcName, null);
          return Cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

		//ｹｫﾓﾐｷｽｷｨ｣ｬｵﾃｴ豢｢ｹｳﾌ(ｴﾎﾊ)
		//ﾊ菠・ｺ
		//			ProcName - ｴ豢｢ｹｳﾌﾃ・
		//      Params	 - ｴ豢｢ｹｳﾌﾐ靨ｪｵﾄｲﾎﾊ
		//ﾊ莎ｺ
		//			ｽｫﾖｴﾐﾐｽ盪鋐ﾔSqlDataReaderｷｵｻﾘ
		//ﾗ｢ﾒ筌ｺﾊｹﾓﾃｺ箏ﾃSqlDataReader.Close()ｷｽｷｨ
        public SqlDataReader RunProcGetReader(string ProcName, SqlParameter[] Params) 
        {
          SqlCommand Cmd = CreateCommand(ProcName, Params);
          return Cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        //ｹｫﾓﾐｷｽｷｨ｣ｬｵﾃｴ豢｢ｹｳﾌ(ｴﾎﾊ)
        //ﾊ菠・ｺ
        //		ProcName - ｴ豢｢ｹｳﾌﾃ・
        //      Params	 - ｴ豢｢ｹｳﾌﾐ靨ｪｵﾄｲﾎﾊ
        //ﾊ莎ｺ
        //			ｽｫﾖｴﾐﾐｽ盪鋐ﾔSqlDataReaderｷｵｻﾘ
        //ﾗ｢ﾒ筌ｺﾊｹﾓﾃｺ箏ﾃSqlDataReader.Close()ｷｽｷｨ
        public int RunProcGetCount(string ProcName, SqlParameter[] Params)
        {
            SqlCommand Cmd = CreateCommand(ProcName, Params);            
            string SCount;            
            SCount = Cmd.ExecuteScalar().ToString().Trim();
            if (SCount == "")
                SCount = "0";
            Close();
            return Convert.ToInt32(SCount);
        }

        //ｹｫﾓﾐｷｽｷｨ｣ｬｵﾃｴ豢｢ｹｳﾌ(ｲｻｴﾎﾊ)
        //ﾊ菠・ｺ
        //			ProcNameｴ豢｢ｹｳﾌﾃ・
        //ﾊ莎ｺ
        //			ｽｫﾖｴﾐﾐｽ盪鋐ﾔDataSetｷｵｻﾘ    
        public DataSet GetDataSet(string ProcName)
        {
            Open();
            SqlDataAdapter adapter = new SqlDataAdapter(ProcName, Connection);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            Close();
            return dataset;
        }

        //ｹｫﾓﾐｷｽｷｨ｣ｬｵﾃｴ豢｢ｹｳﾌ(ｲｻｴﾎﾊ)
        //ﾊ菠・ｺ
        //			ProcNameｴ豢｢ｹｳﾌﾃ・
        //ﾊ莎ｺ
        //			ｽｫﾖｴﾐﾐｽ盪鋐ﾔDataSetｷｵｻﾘ    
        public DataSet GetDataSet(string ProcName, SqlParameter[] Params)
        {
            //Open();
            SqlCommand Cmd = CreateCommand(ProcName, Params);
            SqlDataAdapter adapter = new SqlDataAdapter(Cmd);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);
            Close();
            return dataset;
        }
        //ｹｫﾓﾐｷｽｷｨ｣ｬｸﾝSqlﾓ・茱ｬｷｵｻﾘﾒｻｸ盪鈹ｾﾝｼｯ
        public DataSet GetDataSetSql(string XSqlString)
        {
            Open();
            SqlDataAdapter Adapter = new SqlDataAdapter(XSqlString, Connection);
            DataSet Ds = new DataSet();
            Adapter.Fill(Ds);
            Close();
            return Ds;
        }
        //ｹｫﾓﾐｷｽｷｨ｣ｬｸﾝSqlﾓ・茱ｬｲ衒・ﾇﾂｼ
        public int Insert(string XSqlString)
        {
            int Count = -1;
            Open();
            SqlCommand cmd = new SqlCommand(XSqlString, Connection);
            Count = cmd.ExecuteNonQuery();
            Close();
            return Count;            
        }
        //ｹｫﾓﾐｷｽｷｨ｣ｬｸﾝSqlﾓ・茱ｬｲ衒・ﾇﾂｼｲ｢ｷｵｻﾘﾉ嵭ﾉｵﾄIDｺﾅ
        public int GetIDInsert(string XSqlString)
        {
            int Count = -1;
            Open();
            SqlCommand cmd = new SqlCommand(XSqlString, Connection);
            Count = int.Parse(cmd.ExecuteScalar().ToString().Trim());
            Close();
            return Count;

        }
	
	}
}
