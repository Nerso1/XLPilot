   M S B A M L   `   `   `  ����B  >XLPilot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:XLPilot.UserControlsXLPilot.UserControls  5      0    *XLPilot.UserControls.WrapPanel_DragAndDrop   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:XLPilot.UserControls     5      $	��600��5      $
��Auto��6   .��5      �� -   5        $
��Auto=�5      $��Disabled=�5        �� PanningMode$  VerticalOnly=�5      .��5   
   �  � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5      �� $��0��6   $��Twoje przyciski��6%   $��5q�6<   $��16(�6G   �� 5      $��1��6   
�������$��1q�65   $��5q�6I   ._�5      � .+�5      �� -   5         ��Name$ emptyStateMessage��5       $0��*Przeciągnij wybrane ikony do tego obszaru��5        $��Center=�5!       $��Center=�5"        J� IsHitTestVisible .  
�������$��16(�5%       $��Italic$�5&       ��5'      �� 5(       �� 
TargetType" ��6$   .�5)   "   ��  �� Property � y� �� Value$ 	Collapsed=�6?    �� Triggers 5*   "   �� 5+   &    �� Binding 62   �� *ProjectItems.Count+	 �� Value	 06Y   .a�5,   *   ��  � y�$ Visible=�6G   5-   '   5/      51      �� 53      -   5        
 ��Name$
 projectListView��53      $	��120��54       J� 	AllowDrop . 
������ $��0q�5>      $��Disabled=�5?      ��55      �� *ProjectItems+��5@      �� 5A      " ��6$   .�5B   "   ��  � ��$ Center=�6O   �� 5C   "    � ��$ Center=�6M   �� 5D   "    � ��$ 2q�6;   �� 5E   "    � ��$ 0q�6<   �� 5F   "    � ��
 ���� 6?   �� 5G   "    � ��
 ���� 6@   �� 5H   "    � ��$ 0q�6D   5I      ��5K      �� 5L      .R�5M   "   �  � 	ItemWidth$	 100��6,   $��Center=�6<   5O      ��5Q      �� 5R      .R�5S   "   -   'XLPilot.UserControls.PilotButtonMovable  $	��100��6;   $	��100��6G      ImageSource 5T   :   �� *ImageSource+   
ButtonText 5U   :   �� *
ButtonText+   ToolTipText 5V   :   �� *ToolTipText+5W      5Y      5Z      5[      �� 5^      $��2��6   $��Przyciski do wyboru��6%   $��5q�6@   $��16(�6K   �� 5`      $��3��6   
�������$��1q�65   $��5q�6I   ._�5a      � .+�5c      � -   5         �Name$ trashIndicator��5c      $y�	Collapsed=�63    .   K� ZIndex$	 100��6c   .+�5d      �� 
������� J� Opacity$	 0.7Q�65   �� 5e      $��Center=�6%   $��Center=�6@   .+�5f      �� $��🗑️��6(   $��48(�63   $��Center=�6A   �� 5g      $ ��Upuść tutaj aby usunąć��6(   $��Center=�6F   $��16(�6c   $
��Bold"�6q   5h      5i      �� 5l      -   5        $
 toolboxListView��5l       . 
������ $��0q�5v      $��Disabled=�5w      ��5m      �� *ToolboxItems+��5x      �� 5y      " ��6$   .�5z   "   ��  � ��$ Center=�6O   �� 5{   "    � ��$ Center=�6M   �� 5|   "    � ��$ 2q�6;   �� 5}   "    � ��$ 0q�6<   �� 5~   "    � ��
 ���� 6?   �� 5   "    � ��
 ���� 6@   �� 5�   "    � ��$ 0q�6D   5�      ��5�      �� 5�      .R�5�   "   � $	 100��6,   $��Center=�6<   5�      ��5�      �� 5�      .R�5�   "     $	��100��6;   $	��100��6G    5�   :   �� *ImageSource+ 5�   :   �� *
ButtonText+ 5�   :   �� *ToolTipText+5�      5�      5�      5�      �� 5�      -   5         ��Name$ btnAddNewIcon��5�   "   $��4��6   $��➕ Stwórz nowy przycisk��5�      $��Center=�5�      $��0,10,0,5q�5�      $
��10,5q�5�      5�      5�      5�      5�      