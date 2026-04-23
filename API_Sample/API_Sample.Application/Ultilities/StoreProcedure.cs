using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace API_Sample.Application.Ultilities
{
    public static class StoreProcedure
    {
        public static async Task<List<T>> GetListAsync<T>(string connectionString, string spName, string[] arrParams = null!, object[] arrValues = null!) where T : new()
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null && arrValues != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                    }
                    await cmd.Connection.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    list = await reader.ConvertToListAsync<T>();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<T> GetSingleAsync<T>(string connectionString, string spName, string[] arrParams = null!, object[] arrValues = null!) where T : new()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(spName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null && arrValues != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                    }
                    await cmd.Connection.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                    var list = await reader.ConvertToListAsync<T>();
                    return list.SingleOrDefault()!;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<int> ExecuteNonQueryAsync(string connectionString, string spName, string[] arrParams = null!, object[] arrValues = null!, bool spReturnValue = true)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null && arrValues != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                    }
                    if (spReturnValue)
                    {
                        // Trong Store Procedure dùng lệnh Return một số nguyên có tham số @returnVal
                        var returnValue = new SqlParameter
                        {
                            ParameterName = "@returnVal",
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.ReturnValue
                        };
                        cmd.Parameters.Add(returnValue);
                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return (int)returnValue.Value;
                    }
                    else
                    {
                        await con.OpenAsync();
                        return await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<int> ExecuteNonQuerySingleAsync(string connectionString, string spName, string[] arrParams = null!, object[] arrValues = null!)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null && arrValues != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                    }
                    await con.OpenAsync();
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<int> ExecuteScalarAsync(string connectionString, string spName, string[] arrParams = null!, object[] arrValues = null!)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null && arrValues != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                    }
                    await con.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync();
                    if (result != null && int.TryParse(result.ToString(), out int code))
                        return code;

                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<DbDataReader> GetReaderAsync(string connectionString, string spName, string[] arrParams = null!, object[] arrValues = null!)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null && arrValues != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                    }
                    await cmd.Connection.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    return reader;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<List<T>> ConvertToListAsync<T>(this DbDataReader reader, bool isClose = true) where T : new()
        {
            List<T> list = new List<T>();
            if (reader != null && reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    T obj = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        // Lấy tên cột và giá trị của cột
                        string columnName = reader.GetName(i);
                        object columnValue = reader.GetValue(i);
                        // Tìm thuộc tính tương ứng trong đối tượng T và thiết lập giá trị của nó
                        var property = typeof(T).GetProperty(columnName);
                        if (property != null && columnValue != DBNull.Value)
                        {
                            property.SetValue(obj, columnValue);
                        }
                    }
                    list.Add(obj);
                }
                if (isClose)
                {
                    reader.Close(); // Giải phóng và Close connect
                }
            }
            return list;
        }
    }
}
