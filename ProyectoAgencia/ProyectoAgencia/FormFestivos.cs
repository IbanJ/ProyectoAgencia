using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace ProyectoAgencia
{
    public partial class FormFestivos : Form
    {
        SqlConnection cn = new SqlConnection();

        DataSet ds = new DataSet();
        DataTable dt = new DataTable("Fiestas");
        int x = 0;

        public FormFestivos()
        {
            InitializeComponent();
        }

        private void agrega_fecha(DateTime dfecha, string fiesta)
        {
            Label lbl = new Label();
            lbl.Name = "lblFecha" + x;
            lbl.Text = dfecha.ToShortDateString();
            lbl.Size = new Size(65, 20);
            Point pl = new Point(10, (22 * x) + 4 + 10);
            lbl.Location = pl;
            panelFiestas.Controls.Add(lbl);

            TextBox tb = new TextBox();
            tb.Name = "txtFecha" + x;
            tb.Text = fiesta;
            tb.Size = new Size(180, 20);
            Point pt = new Point(75, (22 * x) + 10);
            tb.Location = pt;
            tb.Leave += new System.EventHandler(this.salirTB);
            panelFiestas.Controls.Add(tb);

            DataRow Linea = dt.NewRow();
            Linea["Fecha"] = dfecha;
            Linea["Fiesta"] = fiesta;
            dt.Rows.Add(Linea);

            CheckBox CKBOX = new CheckBox();
            CKBOX.Name = "ckFecha" + x;
            Point ptck = new Point(265, (22 * x) + 10);
            CKBOX.Location = ptck;
            panelFiestas.Controls.Add(CKBOX);


            x++;
        }
        private void salirTB(object sender, System.EventArgs e)
        {
            // El numero del dtRow es el del mismo textbox que se acaba de salir de escribir
            TextBox tb = sender as TextBox;
            dt.Rows[Convert.ToInt32(tb.Name.Substring(8))]["Fiesta"] = tb.Text;
        }

        private void limpia_form()
        {
            System.Windows.Forms.Control aux;
            int total, drTotal;
            total = dt.Rows.Count - 1;
            for (int i = panelFiestas.Controls.Count - 1; i >= 0; i--)
            {
                aux = panelFiestas.Controls[i];
                if (aux is CheckBox)
                {
                    if (((CheckBox)aux).Checked == true)
                    {
                        drTotal = 0;
                        panelFiestas.Controls.Remove(panelFiestas.Controls[i]);
                        panelFiestas.Controls.Remove(panelFiestas.Controls[i - 1]);
                        panelFiestas.Controls.Remove(panelFiestas.Controls[i - 2]);
                        i = i - 2;

                        List<DataRow> rowsToDelete = new List<DataRow>();
                        foreach (DataRow row in dt.Rows)
                        {
                            if (drTotal == total)
                            {
                                rowsToDelete.Add(row);
                            }
                            drTotal++;
                        }

                        foreach (DataRow row in rowsToDelete)
                        {
                            dt.Rows.Remove(row);
                        }
                    }
                    total--;
                    DataTable dtaux = new DataTable();
                    

                }
            }

            for (int i = 0; i <= panelFiestas.Controls.Count - 1; i++)
            {
                aux = panelFiestas.Controls[i];

                if (aux is System.Windows.Forms.TextBox)
                {
                    if (aux.Name.Substring(0, 8) == "txtFecha")
                    {
                        aux.Name = "txtFecha" + i;
                    }
                }
                if (aux is System.Windows.Forms.Label)
                {
                    if (aux.Name.Substring(0, 8) == "lblFecha")
                    {
                        aux.Name = "lblFecha" + i;
                    }
                }
                else if (aux is System.Windows.Forms.CheckBox)
                {
                    if (aux.Name.Substring(0, 7) == "ckFecha")
                    {
                        aux.Name = "ckFecha" + i;
                    }
                }
            }
            //dt.AcceptChanges();
            //dt.Rows.Clear();
            //x = 0;
        }

        private void calFiestas_DateSelected(object sender, DateRangeEventArgs e)
        {
            bool sw = false;
            DateTime dfecha = e.Start;

            foreach (DataRow lin in dt.Rows)
            {
                if (DateTime.Compare((DateTime)lin["Fecha"], dfecha) == 0)
                {
                    sw = true;
                    break;
                }
            }

            if (!sw && dfecha.Year == DateTime.Now.Year)
            {
                agrega_fecha(dfecha, "");
            }
        }
        private void cmdEliminar_Click(object sender, EventArgs e)
        {
            limpia_form();
        }
        private void FormFestivos_Load(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder ParametrosConexion = new SqlConnectionStringBuilder();

            dt.Columns.Add("Fecha", Type.GetType("System.DateTime"));
            dt.Columns.Add("Fiesta", Type.GetType("System.String"));

            ParametrosConexion.Add("Data Source", "SEGUNDO150");
            ParametrosConexion.Add("Initial Catalog", "AVAN_iban");
            ParametrosConexion.Add("Integrated Security", "SSPI");

            //ParametrosConexion.Add("Data Source", "xxxxxxx,80");
            //ParametrosConexion.Add("Initial Catalog", "Agencia");
            //ParametrosConexion.Add("user", "sa");
            //ParametrosConexion.Add("password", "xxxxxxx");


            cn.ConnectionString = ParametrosConexion.ConnectionString;

            SqlParameter p_anio = new SqlParameter();
            SqlCommand Comando = cn.CreateCommand();
            Comando.CommandType = System.Data.CommandType.Text;
            Comando.CommandText = "select CAST(CAST(Agno AS varchar(4)) + '-' + CAST(Mes AS varchar(2)) + '-' + CAST(DiaMes AS varchar(2)) AS date) Fecha, DescFestivo from Agencia.Calendario where Festivo = 1 and Agno = @anio";
            p_anio.SqlDbType = SqlDbType.SmallInt;
            p_anio.Direction = ParameterDirection.Input;
            p_anio.ParameterName = "@anio";
            p_anio.Value = DateTime.Now.Year;
            Comando.Parameters.Add(p_anio);

            try
            {
                cn.Open();
                SqlDataReader lector = Comando.ExecuteReader();

                while (lector.Read())
                {
                    agrega_fecha((DateTime)((IDataRecord)lector)["Fecha"], (String)((IDataRecord)lector)["DescFestivo"]);
                }
                lector.Close();
            }
            catch (Exception ex)
            {
                //////
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }


        }
        private void cmdEnviar_Click(object sender, EventArgs e)
        {
            SqlCommand Comando = cn.CreateCommand();
            SqlParameter p_tablaFiestas = new SqlParameter();
            try
            {
                p_tablaFiestas.TypeName = "Agencia.tipoFiestas";
                p_tablaFiestas.SqlDbType = SqlDbType.Structured;
                p_tablaFiestas.Direction = ParameterDirection.Input;
                p_tablaFiestas.ParameterName = "@fiestas";

                Comando.CommandText = "Agencia.pa_ImportaFiestas";
                Comando.CommandType = CommandType.StoredProcedure;

                p_tablaFiestas.Value = dt;

                Comando.Parameters.Add(p_tablaFiestas);

                cn.Open();
                Comando.ExecuteNonQuery();

                MessageBox.Show("Fiestas subidas correctamente.", "Fiestas", MessageBoxButtons.OK);
                //limpia_form();
            }
            catch (Exception ex)
            {
                //errores 
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }

        private void panelFiestas_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
