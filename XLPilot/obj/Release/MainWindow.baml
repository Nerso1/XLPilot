   M S B A M L   `   `   `  ���� B  >XLPilot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:XLPilotXLPilot  ;"clr-namespace:XLPilot.UserControlsXLPilot.UserControls  9!clr-namespace:XLPilot.TabControlsXLPilot.TabControls      XLPilot.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:XLPilot     1cc"clr-namespace:XLPilot.UserControls     2tabs!clr-namespace:XLPilot.TabControls       � Title$  XL Pilot��$	��800��$	��620��.��� .+��� -    ��Name$ mainTabControl������  �� 
TargetType" ��.R��  � RowDefinitions �� $
��Auto ��� $��* �.+���  ��Name$ HeaderPanel��$��0�� K� ZIndex$ 1��$��0,0,4,0q� K� IsItemsHost . 
������ ��  ��Name$ ContentPanel��$��1��
�������$��0q�._���  ��Name$ PART_SelectedContentHost��$��SelectedContent������ 	 �� 
TargetType"	 ��.��� 
 �� Property
 � �� �� Value
 ���� �� 
 � ��$ 0q��� 
 � ��$ 0,0,4,0q��� 
 � ��$ 15,10q��� 
 � �� �� " ��.R�� .+��� $ Border��#��z��#��z��#��z�� �� CornerRadius$ 4,4,0,0��._��� $ ContentSite��$��Center=�$��Center=�$��Header��#��z����. �� $ SelectedBorder��$��3��
�������$��Bottom=�$y�	Collapsed=� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���  �� 
TargetName$ Border��
 � �� �#F5F5F5P�  �@��
IsSelected . . ��� $ Border��
 � ��
 ������� $ SelectedBorder��
 � y�$ Visible=��� $ ContentSite��
 � ��
 �   ��� $ ContentSite��
 � ��$ SemiBold"�.@��� $��XLPilot��.��"   XLPilot.TabControls.MainTab1  �� $��Konfiguracja XL-e��.��&    XLPilot.TabControls.XLConfigTab2  �� $��Konfiguracja inne��.��)   #XLPilot.TabControls.OtherConfigTab3  