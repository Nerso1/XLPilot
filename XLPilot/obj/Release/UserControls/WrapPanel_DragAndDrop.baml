   M S B A M L   `   `   `  ���� B  >XLPilot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:XLPilot.UserControlsXLPilot.UserControls  0    *XLPilot.UserControls.WrapPanel_DragAndDrop   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     4local"clr-namespace:XLPilot.UserControls     $	��600��$
��Auto��.���� -   $
��Auto=�$��Disabled=�  �� PanningMode$  VerticalOnly=�.���  � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$��Twoje przyciski��$��5q�$��16(��� $��1��
�������$��1q�$��5q�._�� .+��� -    ��Name$ emptyStateMessage��$0��*Przeciągnij wybrane ikony do tego obszaru��$��Center=�$��Center=� J� IsHitTestVisible .  
�������$��16(�$��Italic$�����  �� 
TargetType" ��.���  �� Property � y� �� Value$ 	Collapsed=� �� Triggers ��  �� Binding �� *ProjectItems.Count+	 �� Value	 0.a���  � y�$ Visible=��� -   
 ��Name$
 projectListView��$	��120�� J� 	AllowDrop . 
������ $��0q�$��Disabled=����� *ProjectItems+���� " ��.���  � ��$ Center=���  � ��$ Center=���  � ��$ 2q���  � ��$ 0q���  � ��
 ���� ��  � ��
 ���� ��  � ��$ 0q����� .R��  � 	ItemWidth$	 100��$��Center=����� .R�-   'XLPilot.UserControls.PilotButtonMovable  $	��100��$	��100��   ImageSource �� *ImageSource+   
ButtonText �� *
ButtonText+   ToolTipText �� *ToolTipText+�� $��2��$��Przyciski do wyboru��$��5q�$��16(��� $��3��
�������$��1q�$��5q�._�� .+�� -    �Name$ trashIndicator��$y�	Collapsed=� .   K� ZIndex$	 100��.+��� 
������� J� Opacity$	 0.7Q��� $��Center=�$��Center=�.+��� $��🗑️��$��48(�$��Center=��� $ ��Upuść tutaj aby usunąć��$��Center=�$��16(�$
��Bold"��� -   $
 toolboxListView�� . 
������ $��0q�$��Disabled=����� *ToolboxItems+���� " ��.���  � ��$ Center=���  � ��$ Center=���  � ��$ 2q���  � ��$ 0q���  � ��
 ���� ��  � ��
 ���� ��  � ��$ 0q����� .R�� $	 100��$��Center=����� .R�  $	��100��$	��100�� �� *ImageSource+ �� *
ButtonText+ �� *ToolTipText+�� -    ��Name$ btnAddNewIcon��$��4��$��➕ Stwórz nowy przycisk��$��Center=�$��0,10,0,5q�$
��10,5q�