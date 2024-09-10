using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
namespace StockMasterSystem1
{
    internal class Conexionbd
    {
        // Cadena de conexión de SQL Server
        string cadenaSQL = "Data Source=DESKTOP-MGPP1T8\\SQLEXPRESS2;Initial Catalog=DBStockMasterSystem; Integrated Security=True";

        // Cadena de conexión de MongoDB
        string cadenaMongoDB = "mongodb+srv://juaneacostar:zpeksvVyT7etRMFp@stockmastersystem.4jg2z.mongodb.net/?retryWrites=true&w=majority&appName=StockMasterSystem";
        // Objetos de conexión SQL Server
        public SqlConnection conectabard = new SqlConnection();

        // Objetos de conexión MongoDB
        private MongoClient mongoClient;
        private IMongoDatabase mongoDatabase;
        private IMongoCollection<BsonDocument> mongoCollection;

        // Constructor
        public Conexionbd()
        {
            // Inicializa conexión de SQL Server
            conectabard.ConnectionString = cadenaSQL;

            // Inicializa conexión de MongoDB
            mongoClient = new MongoClient(cadenaMongoDB);
            mongoDatabase = mongoClient.GetDatabase("DBStockMasterSystem");
            mongoCollection = mongoDatabase.GetCollection<BsonDocument>("productos");
        }

        // Método para abrir conexión a SQL Server
        public void abrirSQL()
        {
            try
            {
                conectabard.Open();
                MessageBox.Show("Conexión a SQL Server abierta", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir SQL Server: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para cerrar conexión a SQL Server
        public void cerrarSQL()
        {
            conectabard.Close();
        }
        public void MigrarDatosSQLaMongo()
        {
            try
            {
                abrirSQL();
                string query = "SELECT * FROM Productos";
                SqlCommand cmd = new SqlCommand(query, conectabard);
                SqlDataReader reader = cmd.ExecuteReader();

                // Crear la colección en MongoDB si no existe
                mongoCollection = mongoDatabase.GetCollection<BsonDocument>("productos");

                while (reader.Read())
                {
                    var documentoMongo = new BsonDocument();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        documentoMongo[reader.GetName(i)] = BsonValue.Create(reader.GetValue(i));
                    }

                    // Suponiendo que "Id" es el campo único que identifica el producto
                    var filtro = Builders<BsonDocument>.Filter.Eq("Id", documentoMongo["Id"]);
                    var documentoExistente = mongoCollection.Find(filtro).FirstOrDefault();

                    // Solo insertar si el documento no existe
                    if (documentoExistente == null)
                    {
                        mongoCollection.InsertOne(documentoMongo);
                    }
                }

                MessageBox.Show("Datos migrados a MongoDB", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                reader.Close();
                cerrarSQL();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al migrar datos a MongoDB: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
