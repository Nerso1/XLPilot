   M S B A M L   `   `   `  ���� B  >XLPilot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:XLPilotXLPilot  ;"clr-namespace:XLPilot.UserControlsXLPilot.UserControls      XLPilot.CreateIconWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:XLPilot     ;userControls"clr-namespace:XLPilot.UserControls       � Title$  Tworzenie nowej ikony��$	��500��$	��550�� � WindowStartupLocation$ CenterOwner=� � 
ResizeMode$ NoResize=� � 	Resources  �� 
TargetType   
FieldLabel �� Property �� Value  BrowseButton��%  &	        '�� [     &	 �     '�� -    '�� }    �� " ��.���  � ��$ 0,10,0,3q���  � ��$ SemiBold"���  � ��$ 14(��� " ��.���  � ��$	 5,3q���  � ��$ 14(���  � ��$ 30���� " ��.���  � ��$ 30����  � ��$	 ...����  � ��$ 5,0,0,0q���  � ��$	 3,0q���  � ��$ 30���� " ��.���  � ��$	 5,3q���  � ��$ 14(���  � ��$ 30���� " ��.���  � ��$ 0,10,0,0q���  � ��$ 14(���  � ��$ Center=�.��� $��20q� � ColumnDefinitions �� $��* ��� $
��Auto �.+�� $��0�� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��Tworzenie nowej ikony��$��18(�$
��Bold"�$��0,0,0,15q��� $��1��	 �� Orientation$	 
Horizontal=�$��0,0,0,15q�.+�	� -   
 	�Name$
 rbXLIcon��$��Ikona XL�� 	� 	GroupName$ IconType��$��0,0,15,0q�$��14(� d� 	IsChecked$
 TrueY�	� -   $
 rbOtherIcon��$��
Ikona Inne��$ IconType��$��14(��� $��2��$"��Plik/folder do uruchomienia:��#��[  � $��3�� �� $��* ��� $
��Auto �.+��� -    ��Name$ txtFilePath��$��0�� �� 
IsReadOnly . �� -    ��Name$ btnBrowseFile��$��1��#��[ �� $��4��$��Nazwa przycisku:��#��[  �� -   $ txtButtonText��$��5���� $��6��$��Obraz ikony:��#��[  � $��7�� �� $��* ��� $
��Auto �.+��� -   $ txtImageSource��$��0���� -   $ btnBrowseImage��$��1��#��[ �� $��8��$��Czy dodać argumenty?��#��[  �� -    ��Name$ cmbArgumentsOption��$��9��.@��� $	��Nie���� $	��Tak���� -	    ��Name$ cbiIncludeDBAndKey��$��Przekaż bazę i klucz���� -
   $ txtArguments��$��10��$��0,10,0,0q�$��60�� �� TextWrapping$
 Wrap=�$	��Top=�{�.  �� -    ��Name$ chkRunAsAdmin��$��11��$ ��Uruchom jako administrator��$��0,15,0,0q�� $��1��$��25,0,0,0q�$	��150�� �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��	Podgląd:��$��16(�$��SemiBold"�$��0,0,0,10q�&    XLPilot.UserControls.PilotButton  -     Name$ previewButton��&    XLPilot.UserControls.PilotButton   	MouseDown$ PreviewButton_MouseDown��$��1��$	��120��$	��120��$��0,10,0,0q��� $��3��$	 
Horizontal=�$��Center=�$��0,20,0,0q�.+��� -   $ 	btnCancel��$��Anuluj��$��70��$��30��$��0,0,5,0q��� -   $ btnOK��$��OK��$��70��$��30��$��5,0,0,0q�{�.  