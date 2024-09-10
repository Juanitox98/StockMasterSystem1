namespace StockMasterSystem1
{
    public partial class Form1 : Form
    {
        // Variable de estado para verificar si la migraci�n se ha realizado
        private static bool migracionRealizada = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Solo realizar la migraci�n si no se ha realizado previamente
            if (!migracionRealizada)
            {
                try
                {
                    // Crear una instancia de la clase Conexionbd
                    Conexionbd conexion = new Conexionbd();

                    // Abrir la conexi�n a SQL Server
                    conexion.abrirSQL();

                    // Migrar los datos de SQL Server a MongoDB
                    conexion.MigrarDatosSQLaMongo();

                    // Cerrar la conexi�n a SQL Server
                    conexion.cerrarSQL();

                    // Marcar la migraci�n como realizada
                    migracionRealizada = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error durante la migraci�n de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
