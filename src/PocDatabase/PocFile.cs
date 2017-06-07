using Newtonsoft.Json;
using PocDatabase.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PocDatabase
{
    public class PocFile<TSchema>
    {
        private const string DB_FOLDER = ".app";
        private const string DB_FILE = "db.json";

        private string fileDb;
        private TSchema _schema;

        private JsonSerializer _serializer = new JsonSerializer
        {
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            Formatting = Formatting.Indented,
        };

        public PocFile()
        {
            if (DevelopmentHelper.IsAttached)
                this.fileDb = Path.Combine(DevelopmentHelper.GetProjectDirectory(), DB_FOLDER, DB_FILE);
            else
                this.fileDb = Path.Combine(Directory.GetCurrentDirectory(), DB_FOLDER, DB_FILE);

            if (File.Exists(fileDb))
                using (StreamReader file = File.OpenText(fileDb))
                    _schema = (TSchema)_serializer.Deserialize(file, typeof(TSchema));

            if (_schema == null)
                _schema = Activator.CreateInstance<TSchema>();
        }

        public List<T> GetCollection<T>()
        {
            var property = _schema.GetType().GetProperties().Where(f => IsCollection<T>(f)).FirstOrDefault();
            List<T> collection = null;
            if (property != null)
            {
                collection = property.GetValue(_schema) as List<T>;
                if (collection == null)
                {
                    collection = new List<T>();
                    property.SetValue(_schema, collection);
                }
            }

            return collection;
        }

        private bool IsCollection<T>(PropertyInfo property)
        {
            if (property.PropertyType.GenericTypeArguments?.Length > 0)
                return property.PropertyType.GenericTypeArguments[0] == typeof(T);
            return false;
        }

        public void Save()
        {
            var fileMode = FileMode.Truncate;

            if (!File.Exists(fileDb))
            {
                FileHelper.CreateFolderIfNeeded(fileDb);
                fileMode = FileMode.CreateNew;
            }

            using (FileStream fs = File.Open(fileDb, fileMode))
            using (StreamWriter sw = new StreamWriter(fs))
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                _serializer.Serialize(jw, _schema);
            }
        }

        public void Drop()
        {
            File.Delete(fileDb);
        }

        public void Truncate()
        {
            File.WriteAllText(fileDb, "");
        }
    }
}
