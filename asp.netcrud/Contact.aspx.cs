using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace asp.netcrud
{
    public partial class Contact : System.Web.UI.Page
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=.\SQLEXPRESS; Initial Catalog=ASPCRUD;Integrated Security=true");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnDelete.Enabled = false;
                filterGridView();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }
        public void Clear()
        {
            hfContactID.Value = "";            
            txtName.Text = txtMobile.Text = txtAddress.Text = "";
            lblSuccessMessage.Text = lblErrorMessage.Text = "";
            btnSave.Text = "Save";
            btnDelete.Enabled = false;

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
            SqlCommand sqlCmd = new SqlCommand("ContactCreateOrUpdate",sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@ContactID",(hfContactID.Value == ""?0:Convert.ToInt32(hfContactID.Value)));
            sqlCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@Mobile",txtMobile.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@Address",txtAddress.Text.Trim());
            sqlCmd.ExecuteNonQuery();
            sqlCon.Close();
            string contactID = hfContactID.Value;
            Clear();
            if(contactID =="")
            {
                lblSuccessMessage.Text = "Salvo com sucesso";
            }else
            {
                lblSuccessMessage.Text = "Dados atualizados";
                filterGridView();
            }
        }
        void filterGridView()
        {
            if(sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
                SqlDataAdapter sqlData = new SqlDataAdapter("ContactViewAll",sqlCon);
                sqlData.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                sqlData.Fill(dt);
                sqlCon.Close();
                gvContact.DataSource = dt;
                gvContact.DataBind();
            }
        }
        protected void lnk_OnClick(object sender,EventArgs e)
        {
            int contactID = Convert.ToInt32((sender as LinkButton).CommandArgument);
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
                SqlDataAdapter sqlData = new SqlDataAdapter("ContactViewByID", sqlCon);
                sqlData.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlData.SelectCommand.Parameters.AddWithValue("@ContactID",contactID);
                DataTable dt = new DataTable();
                sqlData.Fill(dt);
                sqlCon.Close();
                hfContactID.Value = contactID.ToString();
                txtName.Text = dt.Rows[0]["Name"].ToString();
                txtMobile.Text = dt.Rows[0]["Mobile"].ToString();
                txtAddress.Text = dt.Rows[0]["Address"].ToString();
                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
            }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
                SqlCommand slqCmd = new SqlCommand("ContactDeleteByID", sqlCon);
                slqCmd.CommandType = CommandType.StoredProcedure;
                slqCmd.Parameters.AddWithValue("@contactID", Convert.ToInt32(hfContactID.Value));
                slqCmd.ExecuteNonQuery();
                sqlCon.Close();
                Clear();
                filterGridView();
                lblSuccessMessage.Text = "Deletado com sucesso!";
            }
        }
    }
}