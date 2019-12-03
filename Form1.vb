Imports System.IO



Public Class Form1


    <System.Runtime.InteropServices.DllImportAttribute("user32.dll")> _
        Private Shared Function DestroyIcon(ByVal handle _
     As IntPtr) As Boolean
    End Function





    Private dictPath As String = My.Application.Info.DirectoryPath + "\Dictionary.txt"
    Private wordList As New List(Of String)

    ' Boolean flag used to determine when a character 
    ' other than a number is entered.
    Private nonNumberEntered As Boolean = False


    Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        'CreateIcoFile()
    End Sub

    Private Sub CreateIcoFile()
        Dim g As Graphics = Me.CreateGraphics

        Dim bmp As Bitmap = Image.FromFile( _
        "C:\Documents and Settings\Owner\Desktop\wordmaker.png")

        ' Get an Hicon for myBitmap.
        Dim HIcon As IntPtr = bmp.GetHicon()

        'Dim bytes() As Byte = System.Text.Encoding.Unicode.GetBytes(
        Dim s As New IO.MemoryStream
        'Dim str As New IO.FileStream("C:\Documents and Settings\Owner\Desktop\VB Data\wordmaker.ico", FileMode.Create)

        ' Create a new icon from the handle.
        Dim newIcon As Icon = System.Drawing.Icon.FromHandle(HIcon)

        newIcon.Save(s)

        Dim bytes() As Byte '= s.ReadBytes.Length - 1
        bytes = s.ToArray

        Me.Icon = newIcon
        ' Destroy the icon, since the form creates its 
        ' own copy of the icon.
        DestroyIcon(newIcon.Handle)

        My.Computer.FileSystem.WriteAllBytes( _
        "C:\Documents and Settings\Owner\Desktop\VB Data\wordmaker.ico", _
        bytes, False)

        'g.
        '        My.Computer.FileSystem.ReadAllByte(ico.)
        '        'Dim buffer() As Byte = System.Text.Encoding.Unicode.GetBytes(ico)

        'Dim sw As New IO.StreamWriter(My.Computer.FileSystem.SpecialDirectories.Desktop + "\wordmaker.ico")

        'sw.Write(
    End Sub
    'Loads the dictionary that we will use into memory.
    Private Function LoadDict(ByVal dictPath As String) As Boolean
        If File.Exists(dictPath) Then
            Try
                Dim sr As New StreamReader(dictPath)
                Dim s As String = ""

                While Not sr.EndOfStream
                    s = sr.ReadLine

                    If s <> "" And Not s.StartsWith(";") Then
                        wordList.Add(s.Trim)
                    End If
                End While
                sr.Close()
            Catch ex As Exception
                MsgBox(ex.Message)
                Return False
            End Try

        Else
            MsgBox("ERROR:  Can't find dictionary file." + _
            Environment.NewLine + "Press OK to close application.")
            Return False
        End If
        Return True
    End Function


    Private Sub FindWords()
        Dim wrd As String = ""
        Dim found As New List(Of String)
        Dim index As Integer
        Dim tbword As String = tb1.Text

        'Is there a limit on the word length
        If chkbx1.CheckState = CheckState.Checked Then
            If cb1.Text <> "" Then
                rtb1.Text = (Chr(13) + Chr(13) + Chr(13) + Chr(13) + Chr(13) + "     Searching...")
                rtb1.Refresh()
                'Yes, 
                'Limit search to only n-letters.
                For Each word As String In wordList
                    If word.Length = CInt(cb1.Text) Then
                        tbword = tb1.Text

                        For Each c As Char In word
                            If tbword.Contains(c) Then
                                index = tbword.IndexOf(c)
                                tbword = tbword.Remove(index, 1)
                                wrd = wrd + c
                            End If

                            If wrd.Length = word.Length Then
                                found.Add(wrd)
                                Exit For
                            End If
                        Next
                        wrd = ""
                    End If
                Next
            End If

        Else
            rtb1.Text = (Chr(13) + Chr(13) + Chr(13) + Chr(13) + Chr(13) + "     Searching...")
            rtb1.Refresh()
            'No limits on search
            For Each word As String In wordList
                tbword = tb1.Text

                For Each c As Char In word
                    If tbword.Contains(c) Then
                        index = tbword.IndexOf(c)
                        tbword = tbword.Remove(index, 1)
                        wrd = wrd + c
                    End If

                    If wrd.Length = word.Length Then
                        found.Add(wrd)
                        Exit For
                    End If
                Next
                wrd = ""
            Next
        End If

        If found.Count > 0 Then
            rtb1.Text = ""
            For Each word As String In found
                rtb1.AppendText(word + Chr(13))
            Next
        Else
            rtb1.Text = " [No Words Were Found]"
        End If
        found = Nothing
        tb1.Focus()
    End Sub


    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click
        FindWords()
    End Sub

    Private Sub tb1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tb1.TextChanged
        cb1.Items.Clear()

        For i As Integer = 1 To tb1.Text.Length
            cb1.Items.Add(i.ToString)
        Next

        If tb1.Text = "" Then
            btnFind.Enabled = False
            chkbx1.Enabled = False
            chkbx1.CheckState = CheckState.Unchecked
            cb1.Text = ""
        Else
            btnFind.Enabled = True
            chkbx1.Enabled = True
            cb1.Text = tb1.Text.Length
        End If
    End Sub

    Private Sub chkbx1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkbx1.CheckedChanged
        If chkbx1.CheckState = CheckState.Checked Then
            cb1.Enabled = True
        Else
            cb1.Enabled = False
        End If
        tb1.Focus()
    End Sub

    Private Sub btnAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAbout.Click
        AboutDialog.Label1.Text = My.Application.Info.ProductName + " " + My.Application.Info.Version.ToString + _
                Environment.NewLine + My.Application.Info.Copyright
        AboutDialog.ShowDialog()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'If the dictionary is available load it.
        If Not LoadDict(dictPath) Then
            'Else, exit the application
            Me.Close()
        End If
    End Sub

    Private Sub Form1_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        tb1.Focus()
    End Sub


    Private Sub tb1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tb1.KeyDown
        If e.KeyData = Keys.Enter Then
            FindWords()
        End If
    End Sub
End Class
