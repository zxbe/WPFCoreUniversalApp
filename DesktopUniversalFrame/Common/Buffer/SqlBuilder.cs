﻿using DesktopUniversalFrame.Common.MappingAttribute;
using DesktopUniversalFrame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopUniversalFrame.Common.Buffer
{
    /// <summary>
    /// 泛型缓存
    /// 比字典缓存快
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlBuilder<T>
    {
        private static string commandText = null;
        private static SqlOperationType? operationType = null;

        public static string GetSql(SqlOperationType _operationType)
        {
            if(operationType != _operationType)
            {
                operationType = _operationType;
                GetCommandSql();
            }

            return commandText;
        }

        private static void GetCommandSql()
        {
            Type type = typeof(T);
            switch (operationType)
            {
                case SqlOperationType.Select:
                    {
                        string tableName = type.GetAttributeMappingName();
                        string columnName = string.Join(',', type.GetProperties().ExceptKey().Select(p => $"{p.GetAttributeMappingName()}"));
                        commandText = $"select {columnName} from {tableName} where {type.GetProperties().GetKeyInfo().Name} = @id";
                    }
                    break;
                case SqlOperationType.Insert:
                    {
                        string tableName = type.GetAttributeMappingName();
                        string columnString = string.Join(',', type.GetProperties().Select(p => $"{p.Name}"));
                        string valueString = string.Join(",", type.GetProperties().Select(p => $"@{p.Name}"));
                        commandText = $"insert into {tableName} ({columnString}) values ({valueString})";
                    }
                    break;
                case SqlOperationType.Update:
                    {
                        string tableName = type.GetAttributeMappingName();
                        string updateStr = string.Join(',', type.GetProperties().ExceptKey().Select(p => $"{p.Name}=@{p.Name}"));
                        commandText = $"update {tableName} set {updateStr} where id=@id";
                    }
                    break;
                case SqlOperationType.Delete:
                    {
                        string tableName = type.GetAttributeMappingName();
                        commandText = $"delete from {tableName} where id=@id";
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
