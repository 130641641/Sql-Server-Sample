using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class DBase
    {
                /// </summary>
                /// 到数据库执行并返回的方法
                /// </summary>
                /// </param>
                /// 
                public DataTable QueryData(string strSQL)
                {
                    //连接数据库类
                    SqlConnection sqlCon = new SqlConnection();
                    //链接具体的目标
                    sqlCon.ConnectionString = @"Data Source = ETHAN\SQLEXPRESS;  Initial Catalog = MyOnLineExam;Integrated Security=True;";
                    //打开数据
                    try
                    {
                        sqlCon.Open();
                    }
                    catch
                    {
                        //return new DataTable();
                    }

                    //适配器: 执行命令并返回数据
                    SqlDataAdapter da = new SqlDataAdapter(strSQL, sqlCon);
                    //定义一个装载返回数据的容器
                    DataTable dt1 = new DataTable();
                    //把得到的数据放进容器里
                    da.Fill(dt1);
                    //关闭数据
                    sqlCon.Close();
                    //返回数据
                    return dt1;
                }
                /// 
                /// 为保存,删除按钮服务的:对数据库执行命令但不返回数据
                /// </summary>
                /// </param>
                public void Execute(string strSQL)
                {
                    SqlConnection sqlCon = new SqlConnection();
                    sqlCon.ConnectionString = "Data Source = .; User Id = sa; Password = 123456; Initial Catalog = DZBSX_DB";
                    try
                    {
                        sqlCon.Open();
                    }
                    catch
                    {
                        return;
                    }

                    //定义一个只执行的工具
                    SqlCommand cmd = sqlCon.CreateCommand();
                    //为执行工具力量
                    cmd.CommandText = strSQL;
                    //执行
                    cmd.ExecuteNonQuery();
                    sqlCon.Close();

                }


    }
