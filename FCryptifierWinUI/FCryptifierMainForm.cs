using FCryptifier;
using System.Runtime.InteropServices;

namespace FCryptifierUI;

public partial class FCryptifierMainForm : Form
{
    [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
    private static extern bool ZeroMemory(IntPtr Destination, int Length);
    
    private const string AUTHOR = "Gerald Weinberger";
    private const string AUTHOR_EMAIL = "g.weinberger@outlook.com";
    private const string AUTHOR_WEB = "https://github.com/gweinberger/FCryptifier";
    private const int DEV_YEAR = 2025;

    private int pwdCount = 0;

    private List<string> filelist = new List<string>();
    public FCryptifierMainForm()
    {
        InitializeComponent();
    }

    private void FCryptifierMainForm_Load(object sender, EventArgs e)
    {
        if (Environment.GetCommandLineArgs().Length > 1 && File.Exists(Environment.GetCommandLineArgs()[1]))
        {
            filelist.Add(Environment.GetCommandLineArgs()[1]);
            showPassword();
        }
    }

    private void cmdSelectFile_Click(object sender, EventArgs e)
    {
        using OpenFileDialog ofd = new OpenFileDialog()
        {
            ShowHelp = false,
            CheckFileExists = true,
            RestoreDirectory = true,
            Multiselect = true
        };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            filelist = ofd.FileNames.ToList();
            showPassword();
        }
    }

    private void cmdSelectFile_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data == null) return;
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
    }

    private void cmdSelectFile_DragDrop(object sender, DragEventArgs e)
    {
        try
        {
            if (e.Data == null) return;
            object? data = e.Data.GetData(DataFormats.FileDrop);
            if (data == null) return;
            filelist = ((string[])data).ToList();
            showPassword();
        }
        catch
        {
        }
    }

    private void showPassword()
    {
        if (filelist.Count == 0) return;
        cmdSelectFile.Visible = false;
        txtPWD.Visible = true;
        lblPWD.Visible = true;
        txtPWD.Focus();
    }

    private void txtPWD_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter && txtPWD.Text.Length > 0)
        {
            if (isFileToEncrypt(filelist[0]))
            {
                if (pwdCount == 0)
                {
                    pwdCount++;
                    lblPWD.Text = "Repeat";
                    txtPWD.Tag = txtPWD.Text;
                    txtPWD.Text = "";
                    showPassword();
                    return;
                }
                else 
                { 
                    if (txtPWD.Text != txtPWD.Tag?.ToString())
                    {
                        resetUI();
                        MessageBox.Show("Passwords are not equal!", "Wrong input", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
            }
            proceed();
        }
    }

    private void lblInfo_Click(object sender, EventArgs e)
    {
        MessageBox.Show($"Version {Application.ProductVersion}{Environment.NewLine}{Environment.NewLine}©{(DateTime.Now.Year > DEV_YEAR ? DEV_YEAR + "-" : "")}{DateTime.Now.Year} {AUTHOR}{Environment.NewLine}{AUTHOR_EMAIL}{Environment.NewLine}{AUTHOR_WEB}", "FCryptifier");
    }

    private bool isFileToEncrypt(string filename)
    {
        return Path.GetExtension(filelist[0]).ToLower() == ".aes" ? false : true;
    }

    private void resetUI()
    {
        pwdCount = 0;
        txtPWD.Visible = false;
        lblPWD.Visible = false;
        txtPWD.Text = "";
        txtPWD.Tag = "";
        lblPWD.Text = "Password";
        cmdSelectFile.Visible = true;
    }

    private void proceed()
    {
        if (filelist.Count == 0) return;
        try
        {

            bool isSuccessful = false;
            string password = txtPWD.Text;
            string outputFilename = "";
            txtPWD.Visible = false;
            lblPWD.Visible = false;
            txtPWD.Text = "";
            txtPWD.Tag = "";
            Application.DoEvents();

            GCHandle gch = GCHandle.Alloc(password, GCHandleType.Pinned);  // pinning secret is more secure. see: https://stackoverflow.com/questions/20012534/in-c-why-is-pinning-a-secret-key-in-memory-more-secure
            foreach (string file in filelist)
            {
                bool encrypt = isFileToEncrypt(file);
                if (encrypt)
                {
                    outputFilename = file + ".aes";
                }
                else
                {
                    if (Path.GetExtension(file).ToLower() == ".aes")
                    {
                        outputFilename = file.Substring(0, file.LastIndexOf("."));
                    }
                    else
                    {
                        outputFilename = file + "_decrypted";
                    }
                }

                Crypto cr = new(false, true);
                isSuccessful = encrypt ? cr.FileEncrypt(file, outputFilename, password) : cr.FileDecrypt(file, outputFilename, password);
            }
            password = "";
            ZeroMemory(gch.AddrOfPinnedObject(), password.Length * 2);
            gch.Free();
            GC.Collect();

            if (isSuccessful)
            {
                MessageBox.Show($"Successfully completed.{Environment.NewLine}{(filelist.Count == 1 ? "File decrypted: " + outputFilename : "Files decrypted in " + Path.GetDirectoryName(outputFilename))}", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Process aborted.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.ToLower().StartsWith("padding is invalid"))
            {
                MessageBox.Show("Wrong Password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            resetUI();
        }
    }
}
