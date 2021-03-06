﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Microsoft.Win32;

namespace AITool
{
	public static class Global_GUI
	{


		public static void UpdateFOLV(ref FastObjectListView olv, IEnumerable Collection, bool ResizeCols = false)
		{

			olv.Freeze();

			try
			{
				
				olv.ClearObjects();
				olv.SetObjects(Collection, true);
				//update column size only if did not restore folv state file or forced
				if (olv.Tag == null || ResizeCols)
				{
					olv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
				}
				if (olv.Items.Count == 0)
				{
					olv.EmptyListMsg = "Empty";
				}

			}
			catch (Exception ex)
			{
				Global.Log("Error: " + Global.ExMsg(ex));
			}
			finally
			{
				olv.Unfreeze();

			}
		}
		public static void ConfigureFOLV(ref FastObjectListView FOLV, Type Cls, System.Drawing.Font Fnt, ImageList ImageList, string PrimarySortColumnName = "", SortOrder PrimarySortOrder = SortOrder.Ascending, string SecondarySortColumnName = "", SortOrder SecondarySortOrder = SortOrder.Ascending, List<string> FilterColumnList = null, Color Clr = new Color(), int RowHeight = 0, bool ShowGroups = false)
		{

			try
			{

				FOLV.AllowColumnReorder = true;
				FOLV.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
				FOLV.CopySelectionOnControlC = true;
				FOLV.FullRowSelect = true;
				FOLV.GridLines = true;
				FOLV.HideSelection = false;
				FOLV.IncludeColumnHeadersInCopy = true;
				FOLV.OwnerDraw = true;
				FOLV.SelectColumnsOnRightClick = true;
				FOLV.SelectColumnsOnRightClickBehaviour = ObjectListView.ColumnSelectBehaviour.ModelDialog;
				FOLV.SelectedColumnTint = Color.LawnGreen;
				FOLV.ShowCommandMenuOnRightClick = true;
				FOLV.ShowFilterMenuOnRightClick = true;
				FOLV.ShowGroups = false;
				FOLV.ShowImagesOnSubItems = true;
				FOLV.ShowItemCountOnGroups = true;
				FOLV.ShowItemToolTips = true;
				FOLV.ShowSortIndicators = true;
				FOLV.SortGroupItemsByPrimaryColumn = true;
				FOLV.TintSortColumn = true;
				FOLV.UseFiltering = true;
				FOLV.UseHyperlinks = false; //may cause column save/restore error?
				FOLV.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
				FOLV.UseCellFormatEvents = true;
				FOLV.UseNotifyPropertyChanged = true;

				if (ImageList != null)
				{
					FOLV.SmallImageList = ImageList;
				}

				if (Fnt != null)
				{
					FOLV.Font = Fnt;
				}
				else
				{
					FOLV.Font = new Font("Consolas", 8, FontStyle.Regular);
				}

				if (Clr.IsEmpty)
				{
					FOLV.ForeColor = Clr;
				}

				PropertyInfo[] IIProps2 = Cls.GetProperties(); //Cls.GetType().GetProperties


				// Uncomment this to see a fancy cell highlighting while editing
				EditingCellBorderDecoration EC = new EditingCellBorderDecoration(true);
				EC.UseLightbox = true;

				FOLV.AddDecoration(EC);
				FOLV.BuildList();
				int colcnt = 0;

				foreach (PropertyInfo ei in IIProps2)
				{
					if (typeof(IEnumerable).IsAssignableFrom(ei.PropertyType) && !(ei.PropertyType.Name == "String"))
					{
						continue;
					}
					else if (ei.PropertyType.Name == "Object")
					{
						continue;
					}

					colcnt = colcnt + 1;
					OLVColumn cl = new OLVColumn();
					if (ImageList != null)
					{
						//if (FOLV.Name == "FOLV_UpdateList" && colcnt == 1)
						//{
						//	cl.ImageGetter = GetImageForUpdateList;
						//}
						//else if (FOLV.Name == "FOLV_BlocklistViewer" && ei.Name == "RegionalInternetRegistry")
						//{
						//	cl.ImageGetter = GetImageForBlocklistViewerRIR;
						//}
						//else if (FOLV.Name == "FOLV_Apps" && colcnt == 1)
						//{
						//	cl.ImageGetter = GetImageForProdList;
						//}
					}
					//cl.AspectName = ei.Name
					cl.UseFiltering = true;
					cl.Searchable = true;
					cl.Text = ei.Name;
					cl.Name = ei.Name;

					cl.DataType = ei.PropertyType;
					cl.AspectName = ei.Name;

					if (ei.PropertyType.Name == "Int64" || ei.PropertyType.Name == "Int32" || ei.PropertyType.Name == "Timespan")
					{
						cl.TextAlign = HorizontalAlignment.Right;
					}

					if (ei.Name == "UniqueID" || ei.Name == "FoundElementList")
					{
						cl.MinimumWidth = 0;
						cl.MaximumWidth = 0;
						cl.Width = 0;
					}
					else
					{
						cl.MinimumWidth = 50;
						//cl.MaximumWidth = 20000
						//cl.Width = -2
					}

					if (ei.Name.ToLower().Contains("url"))
					{
						cl.Hyperlink = true;
					}
					//If cl.DataType = GetType(Boolean) Then
					//    cl.CheckBoxes = True
					//    cl.IsEditable = False
					//End If


					if (ei.Name.ToLower() == PrimarySortColumnName.ToLower())
					{
						FOLV.PrimarySortColumn = cl;
						FOLV.PrimarySortOrder = PrimarySortOrder; //SortOrder.Descending

						//cl.ImageGetter = AddressOf GetImageFromProd
					}
					if (ei.Name.ToLower() == SecondarySortColumnName.ToLower())
					{
						FOLV.SecondarySortColumn = cl;
						FOLV.SecondarySortOrder = SecondarySortOrder; //SortOrder.Descending
					}

					FOLV.Columns.Add(cl);

				}


				//OLV.RebuildColumns()
				FOLV.Refresh();
				FOLV.BuildList();
				Application.DoEvents();

			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: " + ex.Message);
			}
			finally
			{
				
			}
		}

		public static void RestoreWindowState(Form Frm)
		{

			try
			{
				//private void MyForm_Load(object sender, EventArgs e)
				//{
				//    if (Properties.Settings.Default.IsMaximized)
				//        WindowState = FormWindowState.Maximized;
				//    else if (Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(Properties.Settings.Default.WindowPosition)))
				//    {
				//        StartPosition = FormStartPosition.Manual;
				//        DesktopBounds = Properties.Settings.Default.WindowPosition;
				//        WindowState = FormWindowState.Normal;
				//    }
				//}
				//
				//private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
				//{
				//    Properties.Settings.Default.IsMaximized = WindowState == FormWindowState.Maximized;
				//    Properties.Settings.Default.WindowPosition = DesktopBounds;
				//    Properties.Settings.Default.Save();
				//}

				Point SavLocation = (Point)(GetSetting(Frm.Name + "_Location", new Point())); //Frm.RestoreBounds.Location

				if (SavLocation.IsEmpty)
					goto endOfTry; //we did not previously save settings

				bool SavMaximized = System.Convert.ToBoolean(GetSetting(Frm.Name + "_Maximized", false));
				bool SavMinimized = System.Convert.ToBoolean(GetSetting(Frm.Name + "_Minimized", false));
				object ObjSize = GetSetting(Frm.Name + "_Size", Frm.RestoreBounds.Size);
				Size SavSize = (Size)ObjSize; //CType(ObjSize, System.Drawing.Size)

				//Debug.Print("Size before: " & Me.Size.ToString)
				//Debug.Print("Loc before: " & Me.Location.ToString)
				//Debug.Print("WindowState Before: " & Me.WindowState.ToString)
				//DetectScreenName()

				//============================================================================================================
				//No reliable way of detecting if a screen has been turned off!   This will only detect if fully disconnected
				//============================================================================================================

				Rectangle tstrect = new Rectangle(SavLocation, SavSize);
				if (!SavLocation.IsEmpty && SavLocation.X > 0 && IsVisibleOnAnyScreen(tstrect))
				{
					Frm.Location = SavLocation;
					if (!SavSize.IsEmpty && SavSize.Width > 5 && SavSize.Height > 5)
						Frm.Size = SavSize;
					//else
						//Debug.Print("Saved size not valid.");
				}
				//else
					//Debug.Print("Saved location not valid.");


				if (SavMaximized)
					Frm.WindowState = FormWindowState.Maximized;
				else if (SavMinimized)
					Frm.WindowState = FormWindowState.Normal; //FormWindowState.Minimized

				if (Frm.Tag != null && Frm.Tag.ToString().ToUpper() == "SAVE")
				{
					foreach (Control ctl in Frm.Controls)
						if (ctl.Enabled)
						{
							if (ctl is SplitContainer)
							{
								SplitContainer sc = (SplitContainer)ctl;
								sc.SplitterDistance = System.Convert.ToInt32(GetSetting($"{Frm.Name}.SplitContainer.{sc.Name}.SplitterDistance", sc.SplitterDistance));
							}
							else if (ctl is TabControl)
							{
								TabControl tc = (TabControl)ctl;
								tc.SelectedIndex = System.Convert.ToInt32(GetSetting($"{Frm.Name}.TabControl.{tc.Name}.SelectedIndex", tc.SelectedIndex));
							}
							//else if (ctl is ComboBox)
							//{
							//	ComboBox cc = (ComboBox)ctl;
							//	if (cc.Items.Count == 0 && string.IsNullOrEmpty(System.Convert.ToString(cc.Text)))
							//	{
							//		List<string> lst = new List<string>();
							//		lst = (List<string>)(GetSetting($"{Frm.Name}.ComboBox.{cc.Name}.Items", lst));
							//		foreach (string st in lst)
							//			cc.Items.Add(st);
							//		cc.Text = System.Convert.ToString(GetSetting($"{Frm.Name}.ComboBox.{cc.Name}.Text", cc.Text));
							//	}
							//}
							//else if (ctl is TextBox)
							//{
							//	TextBox tc = (TextBox)ctl;
							//	if (string.IsNullOrEmpty(System.Convert.ToString(tc.Text)))
							//		tc.Text = System.Convert.ToString(GetSetting($"{Frm.Name}.TextBox.{tc.Name}.Text", tc.Text));
							//}
							//else if (ctl is CheckBox)
							//{
							//	CheckBox tc = (CheckBox)ctl;
							//	if (tc.CheckState == CheckState.Indeterminate)
							//	{
							//		tc.Checked = System.Convert.ToBoolean(GetSetting($"{Frm.Name}.CheckBox.{tc.Name}.Checked", tc.Checked));
							//		tc.CheckState = (CheckState)(tc.Checked ? CheckState.Checked : CheckState.Unchecked);
							//	}
							//}
						}

				}
				//If Screen.AllScreens.Any(Function(screen__1) screen__1.WorkingArea.IntersectsWith(Properties.Settings.[Default].WindowPosition)) Then
				//    StartPosition = FormStartPosition.Manual
				//    DesktopBounds = Properties.Settings.[Default].WindowPosition
				//    Frm.WindowState = FormWindowState.Normal
				//End If

				//Debug.Print("Size After: " & Me.Size.ToString)
				//Debug.Print("Loc After: " & Me.Location.ToString)
				//Debug.Print("WindowState After: " & Me.WindowState.ToString)

			}
			catch (Exception)
			{

			}
			finally
			{


			}
endOfTry:
			1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.
		}

		public static void SaveWindowState(Form Frm)
		{


			try
			{

				if (Frm.WindowState == FormWindowState.Maximized)
				{
					SaveSetting(Frm.Name + "_Location", Frm.RestoreBounds.Location);
					SaveSetting(Frm.Name + "_Size", Frm.RestoreBounds.Size);
					SaveSetting(Frm.Name + "_Maximized", true);
					SaveSetting(Frm.Name + "_Minimized", false);
				}
				else if (Frm.WindowState == FormWindowState.Normal)
				{
					SaveSetting(Frm.Name + "_Location", Frm.Location);
					SaveSetting(Frm.Name + "_Size", Frm.Size);
					SaveSetting(Frm.Name + "_Maximized", false);
					SaveSetting(Frm.Name + "_Minimized", false);
				}
				else
				{
					SaveSetting(Frm.Name + "_Location", Frm.RestoreBounds.Location);
					SaveSetting(Frm.Name + "_Size", Frm.RestoreBounds.Size);
					SaveSetting(Frm.Name + "_Maximized", false);
					SaveSetting(Frm.Name + "_Minimized", true);
				}

				if (Frm.Tag != null && Frm.Tag.ToString().ToUpper() == "SAVE")
				{
					foreach (Control ctl in Frm.Controls)
						if (ctl.Enabled)
						{

							if (ctl is SplitContainer)
							{
								SplitContainer sc = (SplitContainer)ctl;
								SaveSetting($"{Frm.Name}.SplitContainer.{sc.Name}.SplitterDistance", sc.SplitterDistance);
							}
							else if (ctl is TabControl)
							{
								TabControl tc = (TabControl)ctl;
								SaveSetting($"{Frm.Name}.TabControl.{tc.Name}.SelectedIndex", tc.SelectedIndex);
							}
							//else if (ctl is ComboBox)
							//{
							//	ComboBox cc = (ComboBox)ctl;
							//	List<string> lst = new List<string>();
							//	foreach (object cbi in cc.Items)
							//		lst.Add(cbi.ToString());
							//	SaveSetting($"{Frm.Name}.ComboBox.{cc.Name}.Items", lst);
							//	SaveSetting($"{Frm.Name}.ComboBox.{cc.Name}.Text", cc.Text);
							//}
							//else if (ctl is TextBox)
							//{
							//	TextBox tc = (TextBox)ctl;
							//	SaveSetting($"{Frm.Name}.TextBox.{tc.Name}.Text", tc.Text);
							//}
							//else if (ctl is CheckBox)
							//{
							//	CheckBox tc = (CheckBox)ctl;
							//	SaveSetting($"{Frm.Name}.CheckBox.{tc.Name}.Checked", tc.Checked);
							//}
						}

				}

			}
			catch (Exception)
			{

			}
			finally
			{

			}
		}

		private static bool IsVisibleOnAnyScreen(Rectangle rect)
		{
			bool Ret = false;
			int I = 0;
			foreach (Screen screen in Screen.AllScreens)
			{
				I++;
				//Debug.Print($"Screen {I}: Name={screen.DeviceName}, Primary={screen.Primary}, Working={screen.WorkingArea}, Bounds={screen.Bounds}");
				if (screen.WorkingArea.IntersectsWith(rect))
				{
					//Debug.Print("...IntersectsWith");
					Ret = true;
				}
			}

			return Ret;
		}

		public static dynamic GetSetting(string Name, object DefaultValue = null, string SubKey = "")
		{

			//regkey is built from CompanyName\ProductName\MajorVersion.MinorVersion
			Version AN = Assembly.GetExecutingAssembly().GetName().Version;
			string Cname = System.Windows.Forms.Application.CompanyName;
			string Pname = System.Windows.Forms.Application.ProductName;
			string version = AN.Major + "." + AN.Minor;
			object RetVal = DefaultValue;
			string SKey = "";
			if (!string.IsNullOrWhiteSpace(SubKey))
				SKey = "\\" + SubKey.Trim();
			try
			{
				string RKey = $"Software\\{Cname}\\{Pname}\\{version}{SKey}"; 

				using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(RKey, false))

					if (reg != null)
					{
						bool Found = false;
						string[] Values = reg.GetValueNames();
						foreach (string valu in Values)
							if (valu.ToLower() == Name.ToLower())
							{
								Found = true;
								RetVal = reg.GetValue(Name, DefaultValue);
								break;
							}
						if (Found)
						{
							if (reg.GetValueKind(Name) == RegistryValueKind.MultiString)
							{
								if (DefaultValue is List<string>)
									RetVal = ((string[])RetVal).ToList();
								else if (DefaultValue is object[])
									RetVal = (string[])RetVal;
								else if (DefaultValue is string[])
									RetVal = (string[])RetVal;
							}
							else if (RetVal is string && DefaultValue is Point)
							{
								//{X=965,Y=399}
								int X = GetNumberInt(Global.GetWordBetween(RetVal.ToString(), "X=", ","));
								int Y = GetNumberInt(Global.GetWordBetween(RetVal.ToString(), "Y=", "}"));
								RetVal = new Point(X, Y);

							}
							else if (RetVal is string && DefaultValue is Size)
							{
								//{Width=931, Height=592}
								int Wid = GetNumberInt(Global.GetWordBetween(RetVal.ToString(), "Width=", ","));
								int Hei = GetNumberInt(Global.GetWordBetween(RetVal.ToString(), "Height=", "}"));
								RetVal = new Size(Wid, Hei);
							}
							else if (DefaultValue != null)
								RetVal = Convert.ChangeType(RetVal, DefaultValue.GetType());
							//Else
							//    RetVal = Convert.ChangeType(RetVal, DefaultValue.GetType)


						}
					}


			}
			catch (Exception ex)
			{
				Global.Log($"Error: {Global.ExMsg(ex)}");
			}

			return RetVal;

		}

		public static int GetNumberInt(object Obj)
		{
			//gets a number from anywhere within a string
			int Ret = 0;
			if (Obj != null)
			{
				if (Obj is string)
				{
					string o = System.Convert.ToString(Obj);
					//If o.Trim.Length > 0 AndAlso o.Trim.Length < 10 Then
					int outint = 0;
					string OnlyNums = System.Convert.ToString(Regex.Match(o, "\\d+").Value);
					if (int.TryParse(OnlyNums, out outint))
						Ret = outint;
					//End If
				}
				else if (Obj is int)
					Ret = (int)Obj;
			}
			return Ret;

		}
		public static bool SaveSetting(string name, object value, string SubKey = "")
		{
			bool ret = false;
			//regkey is built from CompanyName\ProductName\MajorVersion.MinorVersion
			Version AN = Assembly.GetExecutingAssembly().GetName().Version;
			string Cname = System.Windows.Forms.Application.CompanyName;
			string Pname = System.Windows.Forms.Application.ProductName;
			string version = AN.Major + "." + AN.Minor;
			string SKey = "";
			if (!string.IsNullOrWhiteSpace(SubKey))
				SKey = "\\" + SubKey.Trim();
			try
			{
				string RKey = $"Software\\{Cname}\\{Pname}\\{version}{SKey}";
				using (RegistryKey reg = Registry.CurrentUser.CreateSubKey(RKey, RegistryKeyPermissionCheck.ReadWriteSubTree))
					if (reg != null)
					{
						if (value is List<string>)
						{
							List<string> strlist = (List<string>)value;
							reg.SetValue(name, strlist.ToArray(), RegistryValueKind.MultiString);
						}
						else if (value is object[])
						{
							List<string> strlist = new List<string>();
							object[] objects = (object[])value;
							foreach (object obj in objects)
								strlist.Add(obj.ToString());
							reg.SetValue(name, strlist.ToArray(), RegistryValueKind.MultiString);
						}
						else
							reg.SetValue(name, value);
						ret = true;
					}


			}
			catch (Exception ex)
			{
				Global.Log($"Error: {Global.ExMsg(ex)}");
			}
			finally
			{

			}

			return ret;
		}

		//		public string GetImageForProdList(object row)
		//		{
		//			string RetKey = "";

		//			try
		//			{
		//				if (!(row is ClsProdConfEntryShort))
		//				{
		//					//INSTANT C# WARNING: 'Exit Try' statements have no equivalent in C#, so it has been replaced with a 'goto' statement:
		//					//ORIGINAL LINE: Exit Try
		//					goto ExitLabel1;
		//				}

		//				ClsProdConfEntryShort Prod = (ClsProdConfEntryShort)row;
		//				if (Frm_Main.ImageList1.Images.ContainsKey(Prod.Source.ToString().ToLower()))
		//				{
		//					RetKey = Prod.Source.ToString().ToLower();
		//				}
		//				else
		//				{
		//					RetKey = "unknown";
		//				}

		//			}
		//			catch (Exception ex)
		//			{
		//				if (DebugApp)
		//				{
		//					Debug.Print("Error: " + ExMsg(ex));
		//				}
		//			}
		//ExitLabel1:
		//			return RetKey;

		//		}


		public class CursorWait:IDisposable
		{
			public CursorWait(bool appStarting = false, bool applicationCursor = true)
			{
				Cursor.Current = appStarting ? Cursors.AppStarting : Cursors.WaitCursor;
				if (applicationCursor)
				{
					System.Windows.Forms.Application.UseWaitCursor = true;
				}
				System.Windows.Forms.Application.DoEvents();
			}

			public void Dispose()
			{
				Cursor.Current = Cursors.Default;
				System.Windows.Forms.Application.UseWaitCursor = false;
				//Windows.Forms.Application.DoEvents()
			}
		}

	}
}
