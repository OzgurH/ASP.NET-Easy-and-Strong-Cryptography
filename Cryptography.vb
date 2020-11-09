Imports System.Diagnostics
Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Public Class SIFRELEME

    'Change this with your key
   
    Private Shared KEY_64() As Byte = {52, 16, 93, 156, 78, 4, 218, 32}
    Private Shared IV_64() As Byte = {55, 103, 246, 79, 36, 99, 167, 3}

    'TripleDES için 24 byte ve 192 bit anahtar  
    Private Shared KEY_192() As Byte = {42, 16, 93, 156, 78, 4, 218, 32,
            15, 167, 44, 80, 26, 250, 155, 112,
            2, 94, 11, 204, 119, 35, 184, 197}
    Private Shared IV_192() As Byte = {55, 103, 246, 79, 36, 99, 167, 3,
            42, 5, 62, 83, 184, 7, 209, 13,
            145, 23, 200, 58, 173, 10, 121, 222}

    

    Public Shared Function Encode3DES(ByVal value As String) As String



        If value <> "" Then
            value = String2HEX(value)
            Dim DESKey As Byte() = {200, 5, 78, 232, 9, 6, 0, 4}
            Dim DESInitializationVector As Byte() = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}
            Using cryptoProvider = New DESCryptoServiceProvider()
                Using memoryStream = New MemoryStream()
                    Using cryptoStream = New CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(DESKey, DESInitializationVector), CryptoStreamMode.Write)
                        Using writer = New StreamWriter(cryptoStream)
                            writer.Write(value)
                            writer.Flush()
                            cryptoStream.FlushFinalBlock()
                            writer.Flush()
                            Return String2HEX(Convert.ToBase64String(memoryStream.GetBuffer(), 0, CInt(memoryStream.Length)))
                        End Using
                    End Using
                End Using
            End Using
        Else
            Return value
        End If
    End Function


    '3 DES çözme
    Public Shared Function Decode3DES(ByVal value As String) As String


        If value <> "" Then

            value = HttpContext.Current.Server.UrlEncode(value)

            value = value.Replace(" ", "+")
            Dim mod4 As Integer = value.Length Mod 4
            If mod4 > 0 Then
                value += New String("="c, 4 - mod4)
            End If

            value = HEXString2ASCII(value)

            Dim DESKey As Byte() = {200, 5, 78, 232, 9, 6, 0, 4}
            Dim DESInitializationVector As Byte() = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

            Using cryptoProvider = New DESCryptoServiceProvider()
                Using memoryStream = New MemoryStream(Convert.FromBase64String(value))
                    Using cryptoStream = New CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(DESKey, DESInitializationVector), CryptoStreamMode.Read)
                        Using reader = New StreamReader(cryptoStream)
                            Return HEXString2ASCII(reader.ReadToEnd())
                        End Using
                    End Using
                End Using
            End Using
        Else
            Return ""
        End If
    End Function

    
    Public Shared Function String2HEX(ByVal str As String) As String
        Try
            Dim sHEX As String = ""
            str = HttpContext.Current.Server.UrlEncode(str)
            For i As Integer = 0 To str.Length - 1
                sHEX = sHEX + Conversion.Hex(Strings.Asc(str(i)))
            Next
            Return sHEX
        Catch ex As Exception
            Return ""
        End Try
    End Function

   
    Public Shared Function HEXString2ASCII(ByVal hexString As String) As String
        Try
            Dim sb As New StringBuilder()
            For i As Integer = 0 To hexString.Length - 2 Step 2
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), Globalization.NumberStyles.HexNumber))))
            Next
            Return HttpContext.Current.Server.UrlDecode(sb.ToString())
        Catch ex As Exception
            Return ""
        End Try
    End Function


   


End Class
