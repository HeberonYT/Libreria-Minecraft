
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.Win32
Imports System.IO.Compression
Imports fastJSON

Namespace Launcher
	Public Class GameLauncher
        Public Shared Function launch(auth As Minecraft.Auth.AuthData, gameDir As String, versionName As String, extraargs As String, rutajava As String) As [Boolean]
            If Not gameDir.EndsWith(Path.DirectorySeparatorChar) Then
                gameDir += Path.DirectorySeparatorChar
            End If
            Dim launcher_log As String = ""
            Dim logs_folder As String = gameDir & Convert.ToString("logs\")
            If extraargs Is Nothing Then
                extraargs = ""
            End If
            If Not Directory.Exists(logs_folder) Then
                Directory.CreateDirectory(logs_folder)
            End If
            Try
                Dim versionDir As String = (Convert.ToString(gameDir & Convert.ToString("versions\")) & versionName) & "\"
                Dim versionFile As String = (versionDir & versionName) & ".jar"
                Dim assetsDir As String = (Environment.ExpandEnvironmentVariables("%appdata%\MC\MultiMinecraft2\system\launcher\assets\"))
                Dim librariesDir As String = (Environment.ExpandEnvironmentVariables("%appdata%\MC\MultiMinecraft2\system\launcher\libraries\"))
                Dim nativesFolder As String = versionDir & Convert.ToString("natives\")
                Dim assetsIndex As String = Nothing
                Dim mainClass As String = Nothing
                Dim versionArgs As String = Nothing
                Dim libraries As [String]() = Nothing
                Dim inLibraries As [String]() = Nothing
                Dim inLibraries2 As [String]() = Nothing
                Dim javaPath As String = Nothing
                Dim gameArgs As String = ""
                Dim [Inherits] As [Boolean] = False
                Dim [inherits2] As [Boolean] = False
                Dim is64bit As Boolean = Not String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))
                If Directory.Exists(gameDir) AndAlso Directory.Exists(versionDir) AndAlso Directory.Exists(assetsDir) AndAlso Directory.Exists(librariesDir) Then
                    If Not Directory.Exists(nativesFolder) Then
                        Directory.CreateDirectory(nativesFolder)
                    Else
                        Dim downloadedMessageInfo As System.IO.DirectoryInfo = New DirectoryInfo(nativesFolder)
                        For Each file__1 As FileInfo In downloadedMessageInfo.GetFiles()
                            file__1.Delete()
                        Next
                        For Each dir As DirectoryInfo In downloadedMessageInfo.GetDirectories()
                            dir.Delete(True)
                        Next
                        Directory.Delete(nativesFolder)
                        Directory.CreateDirectory(nativesFolder)
                    End If
                    If Not File.Exists((versionDir & versionName) & ".json") Then
                        launcher_log += (Convert.ToString(Convert.ToString("No se ha podido encontrar el archivo JSON de la version indicada: ") & versionDir) & versionName) & ".json" & Environment.NewLine
                        File.WriteAllText(logs_folder & Convert.ToString("launcher.log"), launcher_log)
                        Return False
                    End If
                    Dim versionJson As String = File.ReadAllText((versionDir & versionName) & ".json")
                    Dim versionData As Dictionary(Of [String], [Object]) = DirectCast(JSON.Parse(versionJson), Dictionary(Of [String], [Object]))
                    For Each pair As KeyValuePair(Of [String], [Object]) In versionData
                        Select Case pair.Key
                            Case "inheritsFrom"
                                [Inherits] = True
                                If Not File.Exists((gameDir & Convert.ToString("versions\")) & Convert.ToString(pair.Value) & "\" & Convert.ToString(pair.Value) + ".json") Then
                                    launcher_log += (Convert.ToString("No se ha podido encontrar el archivo JSON de la version heredada: ") & gameDir) & "versions\" & Convert.ToString(pair.Value) & "\" & Convert.ToString(pair.Value) & ".json" & Environment.NewLine
                                    File.WriteAllText(logs_folder & Convert.ToString("launcher.log"), launcher_log)
                                    Return False
                                End If
                                Dim inheritedVersionJSON As String = File.ReadAllText((gameDir & Convert.ToString("versions\")) & Convert.ToString(pair.Value) & "\" & Convert.ToString(pair.Value) & ".json")
                                Dim inheritedData As Dictionary(Of [String], [Object]) = DirectCast(JSON.Parse(inheritedVersionJSON), Dictionary(Of [String], [Object]))
                                For Each inPair As KeyValuePair(Of [String], [Object]) In inheritedData
                                    Select Case inPair.Key
                                        Case "inheritsFrom"
                                            [inherits2] = True
                                            Dim inheritedVersionJSON2 As String = File.ReadAllText((gameDir & Convert.ToString("versions\")) & Convert.ToString(inPair.Value) & "\" & Convert.ToString(inPair.Value) & ".json")
                                            Dim inheritedData2 As Dictionary(Of [String], [Object]) = DirectCast(JSON.Parse(inheritedVersionJSON2), Dictionary(Of [String], [Object]))
                                            For Each inPairIn2 As KeyValuePair(Of [String], [Object]) In inheritedData2
                                                Select Case inPairIn2.Key
                                                    Case "libraries"
                                                        Dim inLibs As List(Of [Object]) = DirectCast(inPairIn2.Value, List(Of [Object]))
                                                        inLibraries2 = New [String](inLibs.Count - 1) {}
                                                        For i As Integer = 0 To inLibs.Count - 1
                                                            For Each inPair2 As KeyValuePair(Of [String], [Object]) In DirectCast(inLibs(i), Dictionary(Of [String], [Object]))
                                                                If Convert.ToString(inPair2.Key).Equals("name") Then
                                                                    Dim split As [String]() = Convert.ToString(inPair2.Value).Split(":"c)
                                                                    Dim path As [String] = split(0).Replace(".", "\")
                                                                    Dim filename As [String] = ""
                                                                    For j As Integer = 1 To split.Length - 1
                                                                        path += "\" & split(j)
                                                                        If j <> 1 Then
                                                                            filename += "-" & split(j)
                                                                        Else
                                                                            filename = split(j)
                                                                        End If
                                                                    Next
                                                                    path += "\" & filename & ".jar"
                                                                    Try
                                                                        If File.Exists(librariesDir & path) Then
                                                                            inLibraries2(i) = librariesDir & path
                                                                        ElseIf File.Exists(librariesDir & path.Replace(filename, filename & "-natives-windows")) Then
                                                                            inLibraries2(i) = librariesDir & path.Replace(filename, filename & "-natives-windows")
                                                                            ZipFile.ExtractToDirectory(inLibraries2(i), nativesFolder)
                                                                        ElseIf File.Exists(librariesDir & path.Replace(filename, filename & "-natives-windows-" & (If(is64bit, "64", "32")))) Then
                                                                            inLibraries2(i) = librariesDir & path.Replace(filename, filename & "-natives-windows-" & (If(is64bit, "64", "32")))
                                                                            ZipFile.ExtractToDirectory(inLibraries2(i), nativesFolder)
                                                                        Else
                                                                            If Not (librariesDir & path).Contains("nightly") Then
                                                                                launcher_log += (Convert.ToString("No se ha podido encontrar la libreria: ") & librariesDir) & path & Environment.NewLine
                                                                            End If
                                                                        End If
                                                                    Catch ex As Exception
                                                                    End Try
                                                                End If
                                                            Next
                                                        Next
                                                        Exit Select
                                                End Select
                                            Next
                                            Exit Select
                                        Case "libraries"
                                            Dim inLibs As List(Of [Object]) = DirectCast(inPair.Value, List(Of [Object]))
                                            inLibraries = New [String](inLibs.Count - 1) {}
                                            For i As Integer = 0 To inLibs.Count - 1
                                                For Each inPair2 As KeyValuePair(Of [String], [Object]) In DirectCast(inLibs(i), Dictionary(Of [String], [Object]))
                                                    If Convert.ToString(inPair2.Key).Equals("name") Then
                                                        Dim split As [String]() = Convert.ToString(inPair2.Value).Split(":"c)
                                                        Dim path As [String] = split(0).Replace(".", "\")
                                                        Dim filename As [String] = ""
                                                        For j As Integer = 1 To split.Length - 1
                                                            path += "\" & split(j)
                                                            If j <> 1 Then
                                                                filename += "-" & split(j)
                                                            Else
                                                                filename = split(j)
                                                            End If
                                                        Next
                                                        path += "\" & filename & ".jar"
                                                        Try
                                                            If File.Exists(librariesDir & path) Then
                                                                inLibraries(i) = librariesDir & path
                                                            ElseIf File.Exists(librariesDir & path.Replace(filename, filename & "-natives-windows")) Then
                                                                inLibraries(i) = librariesDir & path.Replace(filename, filename & "-natives-windows")
                                                                ZipFile.ExtractToDirectory(inLibraries(i), nativesFolder)
                                                            ElseIf File.Exists(librariesDir & path.Replace(filename, filename & "-natives-windows-" + (If(is64bit, "64", "32")))) Then
                                                                inLibraries(i) = librariesDir & path.Replace(filename, filename & "-natives-windows-" + (If(is64bit, "64", "32")))
                                                                ZipFile.ExtractToDirectory(inLibraries(i), nativesFolder)
                                                            Else
                                                                If Not (librariesDir & path).Contains("nightly") Then
                                                                    launcher_log += (Convert.ToString("No se ha podido encontrar la libreria: ") & librariesDir) & path & Environment.NewLine
                                                                End If
                                                            End If
                                                        Catch ex As Exception
                                                        End Try
                                                    End If
                                                Next
                                            Next
                                            Exit Select
                                    End Select
                                Next
                                Exit Select
                            Case "mainClass"
                                mainClass = Convert.ToString(pair.Value)
                                Exit Select
                            Case "assets"
                                assetsIndex = Convert.ToString(pair.Value)
                                Exit Select
                            Case "minecraftArguments"
                                versionArgs = Convert.ToString(pair.Value)
                                Exit Select
                            Case "libraries"
                                Dim libs As List(Of [Object]) = DirectCast(pair.Value, List(Of [Object]))
                                libraries = New [String](libs.Count - 1) {}
                                For i As Integer = 0 To libs.Count - 1
                                    For Each pair2 As KeyValuePair(Of [String], [Object]) In DirectCast(libs(i), Dictionary(Of [String], [Object]))
                                        If Convert.ToString(pair2.Key).Equals("name") Then
                                            Dim split As [String]() = Convert.ToString(pair2.Value).Split(":"c)
                                            Dim path As [String] = split(0).Replace(".", "\")
                                            Dim filename As [String] = ""
                                            For j As Integer = 1 To split.Length - 1
                                                path += "\" & split(j)
                                                If j <> 1 Then
                                                    filename += "-" & split(j)
                                                Else
                                                    filename = split(j)
                                                End If
                                            Next
                                            path += "\" & filename & ".jar"
                                            Try
                                                If File.Exists(librariesDir & path) Then
                                                    libraries(i) = librariesDir & path
                                                ElseIf File.Exists(librariesDir & path.Replace(filename, filename & "-natives-windows")) Then
                                                    libraries(i) = librariesDir & path.Replace(filename, filename & "-natives-windows")
                                                    ZipFile.ExtractToDirectory(libraries(i), nativesFolder)
                                                ElseIf File.Exists(librariesDir & path.Replace(filename, filename & "-natives-windows-" & (If(is64bit, "64", "32")))) Then
                                                    libraries(i) = librariesDir & path.Replace(filename, filename & "-natives-windows-" & (If(is64bit, "64", "32")))
                                                    ZipFile.ExtractToDirectory(libraries(i), nativesFolder)
                                                Else
                                                    If Not (librariesDir & path).Contains("nightly") Then
                                                        launcher_log += (Convert.ToString("No se ha podido encontrar la libreria: ") & librariesDir) & path & Environment.NewLine
                                                    End If
                                                End If
                                            Catch ex As Exception
                                            End Try
                                        End If
                                    Next
                                Next
                                Exit Select
                            Case "jar"
                                versionFile = (gameDir & Convert.ToString("versions\")) & Convert.ToString(pair.Value) & "\" & Convert.ToString(pair.Value) & ".jar"
                                Exit Select
                        End Select
                    Next
                    gameArgs += extraargs
                    gameArgs += (Convert.ToString(" -Djava.library.path=""") & nativesFolder) & ";"""
                    gameArgs += " -cp """
                    For k As Integer = 0 To libraries.Length - 1
                        gameArgs += libraries(k) & ";"
                    Next
                    If [Inherits] Then
                        For k As Integer = 0 To inLibraries.Length - 1
                            gameArgs += inLibraries(k) & ";"
                        Next
                    End If
                    If [inherits2] Then
                        For k As Integer = 0 To inLibraries2.Length - 1
                            gameArgs += inLibraries2(k) & ";"
                        Next
                    End If
                    If Not File.Exists(versionFile) Then
                        launcher_log += (Convert.ToString("No se ha podido entrar el ejecutable (jar) de la version requerida: ") & versionFile) & Environment.NewLine
                        File.WriteAllText(logs_folder & Convert.ToString("launcher.log"), launcher_log)
                        Return False
                    End If
                    gameArgs += versionFile
                    gameArgs += (Convert.ToString(""" ") & mainClass) & " "
                    versionArgs = versionArgs.Replace("${auth_player_name}", auth.getUsername())
                    versionArgs = versionArgs.Replace("${version_name}", versionName)
                    versionArgs = versionArgs.Replace("${game_directory}", gameDir)
                    versionArgs = versionArgs.Replace("${assets_root}", assetsDir)
                    versionArgs = versionArgs.Replace("${game_assets}", assetsDir & Convert.ToString("virtual\legacy\"))
                    versionArgs = versionArgs.Replace("${assets_index_name}", assetsIndex)
                    versionArgs = versionArgs.Replace("${auth_uuid}", auth.getProfileID())
                    versionArgs = versionArgs.Replace("${auth_access_token}", auth.getAccessToken())
                    If auth.getTwitchToken() IsNot Nothing Then
                        versionArgs = versionArgs.Replace("${user_properties}", "{""twitch_access_token"":[""" & auth.getTwitchToken() & """]}")
                    Else
                        versionArgs = versionArgs.Replace("${user_properties}", "{}")
                    End If
                    versionArgs = versionArgs.Replace("${user_type}", "legacy")
                    versionArgs = versionArgs.Replace("${auth_session}", "token:" & auth.getAccessToken() & ":" & auth.getProfileID())
                    gameArgs += versionArgs

                    If rutajava = "" Then
                        Dim javaKey As [String] = "SOFTWARE\JavaSoft\Java Runtime Environment"
                        Using baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(javaKey)
                            Dim currentVersion As [String] = baseKey.GetValue("CurrentVersion").ToString()
                            Using homeKey = baseKey.OpenSubKey(currentVersion)
                                javaPath = homeKey.GetValue("JavaHome").ToString()
                            End Using
                        End Using
                        javaPath += Convert.ToString("\bin\javaw.exe")
                    Else
                        javaPath = rutajava
                    End If

                    launcher_log += "Comando de lanzamiento: " & javaPath & " " & gameArgs & Environment.NewLine
                    Dim proc As New Process()

                    proc.StartInfo.FileName = javaPath
                    proc.StartInfo.Arguments = gameArgs
                    proc.StartInfo.UseShellExecute = False
                    proc.StartInfo.CreateNoWindow = False
                    proc.StartInfo.RedirectStandardOutput = True
                    proc.StartInfo.RedirectStandardError = True
                    proc.Start()
                    proc.BeginOutputReadLine()
                    launcher_log += proc.StandardError.ReadToEnd()
                    proc.WaitForExit()
                    If proc.ExitCode <> 0 Then
                        launcher_log += "El juego no se cerro correctamente: Codigo de salida " & Convert.ToString(proc.ExitCode) & Environment.NewLine
                        File.WriteAllText(logs_folder & Convert.ToString("launcher.log"), launcher_log)
                        Return False
                    End If
                    File.WriteAllText(logs_folder & Convert.ToString("launcher.log"), launcher_log)
                    Return True
                Else
                    launcher_log += "No se han podido encontrar los directorios elementales" & Environment.NewLine
                    File.WriteAllText(logs_folder & Convert.ToString("launcher.log"), launcher_log)
                    Return False
                End If
            Catch e As Exception
                launcher_log += "Error interno de la aplicacion: " & e.ToString() & Environment.NewLine
                File.WriteAllText(logs_folder & Convert.ToString("launcher.log"), launcher_log)
                Return False
            End Try
        End Function
	End Class
End Namespace