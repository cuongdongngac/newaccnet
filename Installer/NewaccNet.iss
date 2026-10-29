; =====================================================================
;  BỘ CÀI NEWACCNET (WPF .NET 10) – Inno Setup 6.3+
;
;  Build: chạy ..\publish_all.bat (tự publish x86/x64 rồi gọi ISCC).
;  Hoặc build tay:
;     ISCC /DAppVersion=1.2.0 NewaccNet.iss            -> 1 setup, tự chọn x86/x64
;     ISCC /DAppVersion=1.2.0 /DArch=x64 NewaccNet.iss -> setup riêng bản x64
;     ISCC /DAppVersion=1.2.0 /DArch=x86 NewaccNet.iss -> setup riêng bản x86
;
;  Chế độ "auto" chọn bản cài theo máy:
;     1. Windows 32-bit                          -> x86
;     2. Máy đã có ACE OLEDB 64-bit              -> x64
;     3. Máy đã có ACE OLEDB 32-bit              -> x86
;     4. Chưa có ACE, Office 32-bit              -> x86  (ACE phải cùng bitness Office)
;     5. Còn lại (Office 64-bit / không Office)  -> x64
;
;  Thành phần đi kèm, chỉ cài khi máy còn thiếu (đúng bitness đã chọn):
;     - .NET 10 Desktop Runtime
;     - Visual C++ 2015-2022 Redistributable
;     - Microsoft Access Database Engine 2016 (ACE OLEDB 16.0)
;
;  Ghi log: NewaccNet-Setup.exe /LOG="%TEMP%\newacc-setup.log"
; =====================================================================

#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif
#ifndef Arch
  #define Arch "auto"
#endif

#define AppName    "New Accounting Program"
#define AppShort   "NewaccNet"
#define AppExe     "NewaccNet.Wpf.exe"
#define Publisher  "Newacc"

#define Root       ".."
#define PubX86     Root + "\Publish\x86"
#define PubX64     Root + "\Publish\x64"
#define PreNet     Root + "\Prerequisites\.NET 10.0"
#define PreVC      Root + "\Prerequisites\Visual C++ Redistributable for Visual Studio 2015-2022"
#define PreAce     Root + "\Prerequisites\Access Database Engine 2016"

; Tên file các bộ cài phụ thuộc – đổi ở đây khi cập nhật phiên bản mới
#define NetX64     "windowsdesktop-runtime-10.0.9-win-x64.exe"
#define NetX86     "windowsdesktop-runtime-10.0.9-win-x86.exe"
#define VcX64      "VC_redist.x64.exe"
#define VcX86      "VC_redist.x86.exe"
#define AceX64     "accessdatabaseengine_X64.exe"
#define AceX86     "accessdatabaseengine.exe"

#if Arch == "auto"
  #define Suffix ""
#else
  #define Suffix "-" + Arch
#endif

[Setup]
; GIỮ NGUYÊN AppId này qua mọi phiên bản để cài đè / nâng cấp đúng
AppId={{870FBEB2-A0A8-4DB0-88E4-3101FDB99CDF}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#Publisher}
VersionInfoVersion={#AppVersion}
DefaultDirName={code:GetInstallDir}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename={#AppShort}-Setup-{#AppVersion}{#Suffix}
SetupIconFile={#Root}\icon.ico
UninstallDisplayIcon={app}\{#AppExe}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
CloseApplications=yes
#if Arch == "x64"
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#elif Arch == "x86"
ArchitecturesAllowed=x86compatible
#else
ArchitecturesAllowed=x86compatible
ArchitecturesInstallIn64BitMode=x64compatible
#endif

[Tasks]
Name: "desktopicon"; Description: "Tạo biểu tượng ngoài Desktop"; Flags: unchecked

[Dirs]
; App ghi appsettings.json, *.json cấu hình và file .accdb ngay trong thư mục cài
; -> cấp quyền sửa cho Users để chạy không cần quyền Admin
Name: "{app}"; Permissions: users-modify

[Files]
; ---------------- Ứng dụng ----------------
; Loại file cấu hình/DB do app tạo khi chạy để không ghi đè dữ liệu người dùng
#if Arch != "x86"
Source: "{#PubX64}\*"; DestDir: "{app}"; Check: UseX64; \
  Excludes: "*.pdb,desktop.ini,\appsettings.json,\DebtBalanceSettings.json,\Template\template.accdb"; \
  Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#PubX64}\Template\template.accdb"; DestDir: "{app}\Template"; Check: UseX64; \
  Flags: onlyifdoesntexist uninsneveruninstall
#endif
#if Arch != "x64"
Source: "{#PubX86}\*"; DestDir: "{app}"; Check: not UseX64; \
  Excludes: "*.pdb,desktop.ini,\appsettings.json,\DebtBalanceSettings.json,\Template\template.accdb"; \
  Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#PubX86}\Template\template.accdb"; DestDir: "{app}\Template"; Check: not UseX64; \
  Flags: onlyifdoesntexist uninsneveruninstall
#endif

; ---------------- Thành phần phụ thuộc (chỉ giải nén khi cần, xem [Code]) ----------------
#if Arch != "x86"
Source: "{#PreNet}\{#NetX64}"; DestDir: "{tmp}"; Flags: dontcopy
Source: "{#PreVC}\{#VcX64}";   DestDir: "{tmp}"; Flags: dontcopy
Source: "{#PreAce}\{#AceX64}"; DestDir: "{tmp}"; Flags: dontcopy
#endif
#if Arch != "x64"
Source: "{#PreNet}\{#NetX86}"; DestDir: "{tmp}"; Flags: dontcopy
Source: "{#PreVC}\{#VcX86}";   DestDir: "{tmp}"; Flags: dontcopy
Source: "{#PreAce}\{#AceX86}"; DestDir: "{tmp}"; Flags: dontcopy
#endif

[Icons]
Name: "{group}\{#AppName}";          Filename: "{app}\{#AppExe}"
Name: "{group}\Gỡ cài {#AppName}";   Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}";    Filename: "{app}\{#AppExe}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExe}"; Description: "Chạy {#AppName}"; Flags: nowait postinstall skipifsilent

[Code]
var
  Use64: Boolean;          { cài bản x64 hay x86 }
  AceFound: Boolean;       { máy đã có ACE OLEDB đúng bitness chưa }
  RestartNeeded: Boolean;  { một thành phần phụ thuộc yêu cầu khởi động lại }

{ ---------- Nhận diện môi trường ---------- }

{ Bitness của Office: 'x86', 'x64' hoặc '' nếu không có Office }
function ReadOfficeBitness(): String;
var
  S: String;
begin
  Result := '';
  if not IsWin64 then Exit;
  { Office Click-to-Run (Microsoft 365, 2019, 2021, 2024) }
  if RegQueryStringValue(HKLM64, 'SOFTWARE\Microsoft\Office\ClickToRun\Configuration', 'Platform', S) then
  begin Result := S; Exit; end;
  { Office bản MSI 2016 / 2013 }
  if RegQueryStringValue(HKLM64, 'SOFTWARE\Microsoft\Office\16.0\Outlook', 'Bitness', S) then
  begin Result := S; Exit; end;
  if RegQueryStringValue(HKLM32, 'SOFTWARE\Microsoft\Office\16.0\Outlook', 'Bitness', S) then
  begin Result := S; Exit; end;
  if RegQueryStringValue(HKLM64, 'SOFTWARE\Microsoft\Office\15.0\Outlook', 'Bitness', S) then
  begin Result := S; Exit; end;
  if RegQueryStringValue(HKLM32, 'SOFTWARE\Microsoft\Office\15.0\Outlook', 'Bitness', S) then
  begin Result := S; Exit; end;
end;

{ Provider OLE DB có đăng ký trong view registry (32 hoặc 64) không }
function ProviderInView(RootKey: Integer; ProgId: String): Boolean;
var
  Clsid: String;
begin
  Result := False;
  if RegQueryStringValue(RootKey, ProgId + '\CLSID', '', Clsid) then
    Result := RegKeyExists(RootKey, 'CLSID\' + Clsid + '\InprocServer32');
end;

{ App dùng Microsoft.ACE.OLEDB.16.0 nên chỉ coi bản 16.0 là đủ }
function AceInView(X64: Boolean): Boolean;
begin
  if not IsWin64 then
    Result := ProviderInView(HKCR, 'Microsoft.ACE.OLEDB.16.0')
  else if X64 then
    Result := ProviderInView(HKCR64, 'Microsoft.ACE.OLEDB.16.0')
  else
    Result := ProviderInView(HKCR32, 'Microsoft.ACE.OLEDB.16.0');
end;

{ .NET 10 Desktop Runtime đúng bitness đã có chưa }
function HasDesktopRuntime(X64: Boolean): Boolean;
var
  Base: String;
  FR: TFindRec;
begin
  if X64 then
    Base := ExpandConstant('{commonpf64}')
  else
    Base := ExpandConstant('{commonpf32}');
  Base := Base + '\dotnet\shared\Microsoft.WindowsDesktop.App\';
  Result := FindFirst(Base + '10.*', FR);
  if Result then FindClose(FR);
end;

function InitializeSetup(): Boolean;
var
  Office: String;
  Ace64, Ace32: Boolean;
begin
  Result := True;
  Office := ReadOfficeBitness();
  Ace64 := IsWin64 and AceInView(True);
  Ace32 := AceInView(False);
  Log(Format('Win64=%d, ACE64=%d, ACE32=%d, Office="%s"', [Ord(IsWin64), Ord(Ace64), Ord(Ace32), Office]));

#if Arch == "x64"
  Use64 := True;
#elif Arch == "x86"
  Use64 := False;
#else
  if not IsWin64 then
    Use64 := False
  else if Ace64 then
    Use64 := True
  else if Ace32 then
    Use64 := False
  else
    Use64 := (CompareText(Office, 'x86') <> 0);
#endif

  if Use64 then AceFound := Ace64 else AceFound := Ace32;

  { Bộ cài cố định bitness mà Office khác bitness -> ACE sẽ không cài được }
  if (not AceFound) and (Office <> '') and
     ((Use64 and (CompareText(Office, 'x86') = 0)) or ((not Use64) and (CompareText(Office, 'x64') = 0))) then
  begin
    if SuppressibleMsgBox(
         'Máy này đang dùng Office ' + Office + ', khác bitness với bộ cài này.' + #13#10 +
         'Access Database Engine sẽ không cài được và chương trình không mở được file .accdb.' + #13#10#13#10 +
         'Nên dùng bộ cài tự chọn (không có hậu tố -x86/-x64). Vẫn tiếp tục?',
         mbConfirmation, MB_YESNO, IDNO) = IDNO then
    begin
      Result := False;
      Exit;
    end;
  end;

  if Use64 then Log('-> Cài bản x64') else Log('-> Cài bản x86');
end;

function UseX64(): Boolean;
begin
  Result := Use64;
end;

function GetInstallDir(Param: String): String;
begin
  if Use64 then
    Result := ExpandConstant('{commonpf64}\{#AppName}')
  else
    Result := ExpandConstant('{commonpf32}\{#AppName}');
end;

{ ---------- Cài thành phần phụ thuộc ---------- }

function RunPrereq(FileName, Params, Title: String; var Code: Integer): Boolean;
begin
  WizardForm.StatusLabel.Caption := 'Đang cài ' + Title + '...';
  ExtractTemporaryFile(FileName);
  Result := Exec(ExpandConstant('{tmp}\') + FileName, Params, '', SW_HIDE, ewWaitUntilTerminated, Code);
  Log(Format('%s: exec=%d, exit code=%d', [FileName, Ord(Result), Code]));
  if Code = 3010 then RestartNeeded := True;
end;

procedure ReportFail(Title: String; Code: Integer; Hint: String);
begin
  SuppressibleMsgBox(
    'Không cài được ' + Title + ' (mã lỗi ' + IntToStr(Code) + ').' + #13#10 + Hint,
    mbError, MB_OK, IDOK);
end;

procedure InstallPrereqs();
var
  Code: Integer;
  Ok: Boolean;
  Bits: String;
begin
  if Use64 then Bits := 'x64' else Bits := 'x86';

  { 1. .NET 10 Desktop Runtime }
  if HasDesktopRuntime(Use64) then
    Log('.NET 10 Desktop Runtime ' + Bits + ' đã có')
  else
  begin
    if Use64 then
      Ok := RunPrereq('{#NetX64}', '/install /quiet /norestart', '.NET 10 Desktop Runtime (x64)', Code)
    else
      Ok := RunPrereq('{#NetX86}', '/install /quiet /norestart', '.NET 10 Desktop Runtime (x86)', Code);
    if not Ok or not ((Code = 0) or (Code = 1638) or (Code = 3010)) then
      ReportFail('.NET 10 Desktop Runtime ' + Bits, Code,
        'Chương trình sẽ không chạy được. Hãy cài tay từ thư mục Prerequisites.');
  end;

  { 2. Visual C++ 2015-2022 (luôn chạy; nếu đã có bản bằng/mới hơn thì trả về 1638) }
  if Use64 then
    Ok := RunPrereq('{#VcX64}', '/install /quiet /norestart', 'Visual C++ Runtime (x64)', Code)
  else
    Ok := RunPrereq('{#VcX86}', '/install /quiet /norestart', 'Visual C++ Runtime (x86)', Code);
  if not Ok or not ((Code = 0) or (Code = 1638) or (Code = 3010)) then
    ReportFail('Visual C++ Runtime ' + Bits, Code, 'Hãy cài tay từ thư mục Prerequisites.');

  { 3. Access Database Engine 2016 }
  if AceFound then
    Log('ACE OLEDB 16.0 ' + Bits + ' đã có')
  else
  begin
    if Use64 then
      Ok := RunPrereq('{#AceX64}', '/quiet', 'Microsoft Access Database Engine (x64)', Code)
    else
      Ok := RunPrereq('{#AceX86}', '/quiet', 'Microsoft Access Database Engine (x86)', Code);
    if not Ok or not ((Code = 0) or (Code = 3010)) or not AceInView(Use64) then
      ReportFail('Microsoft Access Database Engine ' + Bits, Code,
        'Thường do máy có Office khác bitness hoặc Office Click-to-Run.' + #13#10 +
        'Chương trình vẫn được cài nhưng sẽ không mở được file .accdb.');
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssInstall then
    InstallPrereqs();
end;

function NeedRestart(): Boolean;
begin
  Result := RestartNeeded;
end;
