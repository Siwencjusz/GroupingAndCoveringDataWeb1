   M S B A M L   `   `   `  ���� R  NGroupingAndCoveringData, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:LoadingPanelSample.ControlsLoadingPanelSample.Controls  A =Models, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE/clr-namespace:Models.ViewModels;assembly=ModelsModels.ViewModels (    "GroupingAndCoveringData.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     >controls)clr-namespace:LoadingPanelSample.Controls     F
viewModels/clr-namespace:Models.ViewModels;assembly=Models       � Title$   Generator pokryć zbiorów��$	��535��$	��850��$	��535��$	��775��$	��535��$	��780��.��� -   $��MainGrid��
������� � ColumnDefinitions �� $
��115* ��� $	��11* ��� $	��26* � � RowDefinitions �� $	��20* ��� $	��10* �.+�� -   $��	ParamGrid��
�������$��0��$��3�� �� �� �� �� �� ��  �� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� $��2q� �� Orientation$ 
Horizontal=�$��0��$��0��$��Center=�$��Center=�.+��� -    ��Name$ ButtonReadFile��$��Otwórz plik��$	��100��$��35������ *CmdReadFile+�� -   $��
TxtRawData��$��0��$��3��$��3��$��1��" �� VerticalScrollBarVisibility$ Visible=�$��2q�$��Stretch=� �� 
IsReadOnly . ���� *RawDataString+ �� UpdateSourceTrigger$ PropertyChanged=� �� TargetNullValue$  ���� $��3��$��3��$ 
Horizontal=�$��1��$
��Left=�$	��Top=�.+��� -   	 ��Name$	 LabellowInput��.���� .+��� $	��LOW���� -   
 ��Name$
 
TextBoxLOW��   ValidationTemplate =� ErrorTemplate# [    TextBoxInError#��[ $��20��$	��120��{��� *RunButtonIndicatorVisible+$ PropertyChanged=�.������  �� Path$	 LOW�  	Converter �� 	Converter# [ $ PropertyChanged=�" �� ValidatesOnNotifyDataErrors .  �� ValidatesOnExceptions .  �� ValidatesOnDataErrors .  �� StringFormat$ {0:#,#.00}}��.���� -   $	 LabelHIGHInput��.���� .+��� $
��HIGH���� -   $
 TextBoxHIGH��$��20��$	��120��# [  #��[ {��� *RunButtonIndicatorVisible+$ PropertyChanged=�.������ $
 HIGH�# [ $ {0:#,#.00}}��$ PropertyChanged=� .  .  . .���� $	��0,6q�{��� *RunButtonIndicatorVisible+$ PropertyChanged=�.+��� -	    ��Name$ CheckBox��$
��Krok�� d� 	IsChecked �� *	UseStep+$ PropertyChanged=��� -
   $
 TextBoxStep��# [  #��[ $��20��$	��120��{��� *	UseStep+.������ $
 Step�# [ $ {0:#,#.00}}��$ PropertyChanged=� .  .  . .���� -   $	 LabelhighInput1��$��2��$��3��$��2��$
��Left=�$	��Top=�.���� .+��� $��Metoda grupowania���� -    ��Name$ GroupingMethods��$��20��$	��220�� �� DisplayMemberPath$ 
MethodName�� �� SelectedIndex$ 0��{��� *RunButtonIndicatorVisible+$ PropertyChanged=����� *MethodsDelegates+ �� SelectedItem �� *SelectedMethod+�� -   $	 ParamInputLabel��$��-2q�$��2��$��5��$
��Left=�$	��Top=�.���� .+��� ���� *ParamInputText+�� -   $
 
ParamInput��# [  #��[ $��20��$	��120��{��� *RunButtonIndicatorVisible+$ PropertyChanged=�.������ $ 
ParamInput�# [ $ PropertyChanged=� �� Mode$ TwoWay=� .  .  . $ {0:00}}��.���� $��3��$��3��$ 
Horizontal=�$��3��$��Right=�$	��Top=�.+�� .+�� -   $��pbStatus��$	��215��$��35�� � Minimum$ 0Q� � Maximum$	 100Q�y��� *RunIndicatorVisible+   BooleanToVisibilityConverter# [ $ PropertyChanged=� � Value �� *Progress.Progress+�� $��Center=�$��Center=�y��� *RunIndicatorVisible+# [ $ PropertyChanged=�����  �� ElementName$ pbStatus��$ Value�$ 	{0:#.00}%���� -   $ 	ButtonRun��$��5q�$��Uruchom��$	��100��$��35��y��� *RunButtonIndicatorVisible+# [ $ PropertyChanged=����� *CmdComputeData+�� -   $ ButtonCancel��$��5q�$��Anuluj��$	��100��$��35��y��� *RunIndicatorVisible+# [ $ PropertyChanged=����� *CmdCancelComputeData+5   /LoadingPanelSample.Controls.CircularProgressBar  -     Name$ progressBarRun��$
��Left=�$	��Top=�$��45��$��5q�$��40��y��� *RunIndicatorVisible+# [ $ PropertyChanged=��� $��5��$��1q�$ 
Horizontal=�$��4��$
��Left=�$��Bottom=�.+��� -   $	 Labellow�� �� ContentStringFormat$ LOW:{0}��$
��Left=�$��5q����� *CoverMatrixlow+�� -   $	 	Labelhigh��$ LOW:{0}��$
��Left=�$��5q����� *CoverMatrixhigh+�� -   $ TrenToExcell��$��Eksport txt treningowe��$	��130��$��30��$��5q����� *CmdExportTrnToTxt+�� -   $ TstToExcell��$��Eksport txt testowe��$	��120��$��30��$��5q����� *CmdExportTstToTxt+�� -   $ ButtonToExcell��$��Eksport do Excela��$	��100��$��30��$��5q����� *CmdExportToExcel+  -   $ progressBarExcell��$
��Left=�$	��Top=�$��30��$��5q�$��40��y��� *ExcelIndicatorVisible+# [ $ PropertyChanged=�&   System.Windows.Controls.DataGrid  -     Name$ DataGrid��$��1��$��3��    
IsReadOnly  . !   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$! 70 $��Stretch=�$��2q����� *CoverMatrixDataTable+