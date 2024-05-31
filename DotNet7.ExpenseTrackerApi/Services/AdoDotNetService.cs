using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace DotNet7.ExpenseTrackerApi.Services;

public class AdoDotNetService
{
    private readonly IConfiguration _configuration;

    public AdoDotNetService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    #region Query
    public List<T> Query<T>(string query, SqlParameter[]? parameters = null)
    {
        SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
        conn.Open();
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddRange(parameters);
        DataTable dt = new();
        SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);
        conn.Close();

        string jsonStr = JsonConvert.SerializeObject(dt);
        List<T> lst = JsonConvert.DeserializeObject<List<T>>(jsonStr)!;

        return lst;
    }
    #endregion

    #region Query With Transaction

    public List<T> Query<T>(SqlConnection conn, SqlTransaction transaction, string query, SqlParameter[]? parameters = null)
    {
        SqlCommand cmd = new(query, conn, transaction);
        cmd.Parameters.AddRange(parameters);
        DataTable dt = new();
        SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);

        string jsonStr = JsonConvert.SerializeObject(dt);
        List<T> lst = JsonConvert.DeserializeObject<List<T>>(jsonStr)!;

        return lst;
    }

    #endregion

    #region Query First Or Default

    public DataTable QueryFirstOrDefault(string query, SqlParameter[]? parameters = null)
    {
        SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
        conn.Open();
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddRange(parameters);
        DataTable dt = new();
        SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);
        conn.Close();
        return dt;
    }

    #endregion

    #region Query First Or Default With Transaction

    public DataTable QueryFirstOrDefault(SqlConnection conn, SqlTransaction transaction, string query, SqlParameter[]? parameters = null)
    {
        SqlCommand cmd = new(query, conn, transaction);
        cmd.Parameters.AddRange(parameters);
        DataTable dt = new();
        SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);
        return dt;
    }

    #endregion

    #region Execute

    public int Execute(string query, SqlParameter[]? parameters = null)
    {
        SqlConnection conn = new(_configuration.GetConnectionString("DbConnection"));
        conn.Open();
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddRange(parameters);
        int result = cmd.ExecuteNonQuery();
        conn.Close();

        return result;
    }

    #endregion

    #region Execute With Transaction

    public int Execute(SqlConnection conn, SqlTransaction transaction, string query, SqlParameter[]? parameters = null)
    {
        SqlCommand cmd = new(query, conn, transaction);
        cmd.Parameters.AddRange(parameters);
        int result = cmd.ExecuteNonQuery();

        return result;
    }

    #endregion
}