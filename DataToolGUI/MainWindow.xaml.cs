using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DataToolLog;

namespace DataTool
{
    public partial class MainWindow
    {
	    public ObservableCollection<DataToolFile> DataTools;

	    private RoutedCommand ReadInputCommand;
        public MainWindow()
        {
            InitializeComponent();
            DataTools = new ObservableCollection<DataToolFile>();
            list.ItemsSource = DataTools;
            LogHelper.DefaultLog.ExtraLog += (log=>this.Dispatcher.BeginInvoke(new Action(delegate { RichTextBox.AppendText(log);RichTextBox.ScrollToEnd();})));
        }


		private void MainWindow_OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Link;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}

		private void MainWindow_OnDrop(object sender, DragEventArgs e)
		{
			var file = ((System.Array) e.Data.GetData(DataFormats.FileDrop))?.GetValue(0).ToString();
			LogHelper.DefaultLog.WriteLine($@"正在读取 [{file}]...");
			DataToolFile dataTool = null;
			BackgroundWorker backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += (a,b) =>
			{
				SetButtonStatus(false);
				try
				{
					if (File.Exists(file))
					{
						dataTool = DataToolFile.GetDataToolFileByZipFile(new FileInfo(file));
					}
					else if (Directory.Exists(file))
					{
						dataTool = DataToolFile.GetDataToolFileByDirectory(new DirectoryInfo(file));
					}
					LogHelper.DefaultLog.WriteLine($@"读取完成");
					
					Dispatcher.BeginInvoke(new Action(delegate { DataTools.Add(dataTool);}));
				}
				catch (Exception exception)
				{
					LogHelper.DefaultLog.WriteLine($@"你写的XML有问题吖 读取失败咯");
					LogHelper.DefaultLog.WriteLine(exception.ToString());
					throw;
				}
				finally
				{
					SetButtonStatus(true);
				}
			};
			backgroundWorker.RunWorkerAsync(null);
		}

		private void ReadInput_OnClick(object sender, RoutedEventArgs e)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].ReadInput();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}
		private void WriteInput_OnClick(object sender, RoutedEventArgs e)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].WriteInput();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}
		private void ReadOutput_OnClick(object sender, RoutedEventArgs e)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].ReadOutput();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}
		private void WriteOutput_OnClick(object sender, RoutedEventArgs e)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].WriteOutput();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}
		private void ConsistencyCheck_OnClick(object sender, RoutedEventArgs e)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].CheckConsistency();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}
		private void OutputGenerate_OnClick(object sender, RoutedEventArgs e)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].GenerateOutput();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}
		private void CaseTest_OnClick(object sender, RoutedEventArgs e)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].TestCase();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}
		private void List_OnSelectionChanged(object paramSender, SelectionChangedEventArgs paramE)
		{
			var datatool = paramE.AddedItems[0] as DataToolFile;
			ArgStackPanel.Children.Clear();

			foreach (var require in datatool.GetRequiredVariable())
			{
				var stackPanel = new StackPanel();
				ArgStackPanel.Children.Add(stackPanel);
				stackPanel.Orientation = Orientation.Horizontal;
				stackPanel.Margin = new Thickness(5);

				var label = new Label();
				label.Foreground = new SolidColorBrush(Colors.Azure);
				label.Content = require.Key;
				stackPanel.Children.Add(label);

				var textBox = new TextBox();
				textBox.Name = require.Value.Item1;
				textBox.Text = require.Value.Item2;
				textBox.Width = 700;
				stackPanel.Children.Add(textBox);
			}
		}

		private void SetButtonStatus(bool status)
		{
			Dispatcher.BeginInvoke(
				new Action(delegate { Button_ReadInput.IsEnabled = status;
					Button_WriteInput.IsEnabled = status;
					Button_ReadOutput.IsEnabled = status;
					Button_WriteOutput.IsEnabled = status;
					Button_ConsistencyCheck.IsEnabled = status;
					Button_OutputGenerate.IsEnabled = status;
					Button_CaseTest.IsEnabled = status;})
				);
		}

		private void Confirm_OnClick(object paramSender, RoutedEventArgs paramE)
		{
			var dictionary = new Dictionary<string, string>();
			var index = list.SelectedIndex;
			if (index>=0)
			{
				foreach (UIElement child in ArgStackPanel.Children)
				{
					StackPanel stackPanel = child as StackPanel;
					TextBox textBox = stackPanel.Children.OfType<TextBox>().Single();
					dictionary.Add(textBox.Name,textBox.Text);
				}
				DataTools[index].SetRequiredVariable(dictionary);
			}
		}

		private void AllGenerate_OnClick(object paramSender, RoutedEventArgs paramE)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].ReadInput();
						DataTools[index].WriteInput();
						DataTools[index].CheckConsistency();
						DataTools[index].GenerateOutput();
						DataTools[index].WriteOutput();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
						LogHelper.DefaultLog.WriteLine($@"执行结束!");
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}

		private void AllTest_OnClick(object paramSender, RoutedEventArgs paramE)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					try
					{
						DataTools[index].ReadInput();
						DataTools[index].ReadOutput();
						DataTools[index].TestCase();
					}
					catch (Exception exception)
					{
						LogHelper.DefaultLog.WriteLine(exception.ToString());
					}
					finally
					{
						SetButtonStatus(true);
						LogHelper.DefaultLog.WriteLine($@"执行结束!");
					}
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}

		private void Serialize_OnClick(object paramSender, RoutedEventArgs paramE)
		{
			var index = list.SelectedIndex;
			if (index>=0)
			{
				BackgroundWorker backgroundWorker = new BackgroundWorker();
				backgroundWorker.DoWork += (a,b) =>
				{
					SetButtonStatus(false);
					DataTools[index].Serialize();
					SetButtonStatus(true);
					LogHelper.DefaultLog.WriteLine($@"执行结束!");
				};
				backgroundWorker.RunWorkerAsync(null);
			}
			else
			{
				LogHelper.DefaultLog.WriteLine($@"index {index}");
			}
		}

		private void MainWindow_OnMouseMove(object paramSender, MouseEventArgs paramE)
		{
			if (paramE.LeftButton==MouseButtonState.Pressed)
			{
				this.DragMove();
			}
		}

		private void UIElement_OnMouseDown(object paramSender, MouseButtonEventArgs paramE)
		{
			this.Close();
		}
    }
}