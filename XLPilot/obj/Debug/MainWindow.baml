   M S B A M L   `   `   `  ����B  >XLPilot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:XLPilotXLPilot  5   	   ;"clr-namespace:XLPilot.UserControlsXLPilot.UserControls  5   	   9!clr-namespace:XLPilot.TabControlsXLPilot.TabControls  5   	       XLPilot.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:XLPilot     5   	   1cc"clr-namespace:XLPilot.UserControls     5   	   2tabs!clr-namespace:XLPilot.TabControls     5   	     � Title$  XL Pilot��5
   	   $	��800��6   $	��620��6'   .��5      � .+�5   
   �� -   5         ��Name$ mainTabControl��5      ��5      �� 5       �� 
TargetType" ��6"   .R�5      �  � RowDefinitions 5      �� 5      $
��Auto �6,   �� 5      $��* �6,   .+�5      ��  ��Name$ HeaderPanel��6#   $��0��5   "    K� ZIndex$ 1��5   "   $��0,0,4,0q�5   "    K� IsItemsHost . 
������ 5   "   �� 5        ��Name$ ContentPanel��6!   $��1��5!   !   
�������$��0q�5#   !   ._�5$      ��  ��Name$ PART_SelectedContentHost��6/   $��SelectedContent��5%   .   5&      5'      5(      ��5,      �� 5-      	 �� 
TargetType"	 ��6   .�5.      �� 
 �� Property
 � �� �� Value
 ���� 63   �� 5/      
 � ��$ 0q�68   �� 50      
 � ��$ 0,0,4,0q�6/   �� 51      
 � ��$ 15,10q�60   �� 52      
 � �� 53      �� 54      " ��6.   .R�55   "   � .+�56   &   �� $ Border��6-   #��z��57   ,   #��z��58   ,   #��z��59   ,    �� CornerRadius$ 4,4,0,0��5:   ,   ._�5<   *   �� $ ContentSite��6;   $��Center=�5=   :   $��Center=�5>   :   $��Header��5?   :   #��z��5@   :   ��. 5A   :   5B   '   �� 5D   &   $ SelectedBorder��6-   $��3��5E   ,   
�������$��Bottom=�5G   ,   $y�	Collapsed=�5H   ,   5I   #    �� Triggers 5J   "   P� 5L   &    P� Property �@J�IsMouseOver P� Value . . �5M   *   ��  �� 
TargetName$ Border��61   
 � �� �#F5F5F56[   5N   '   P� 5Q   &    �@��
IsSelected . . �5R   *   �� $ Border��61   
 � ��
 �����6[   �� 5S   *   $ SelectedBorder��61   
 � y�$ Visible=�6c   �� 5U   *   $ ContentSite��61   
 � ��
 �   �6l   �� 5V   *   $ ContentSite��61   
 � ��$ SemiBold"�6l   5W   '   5Y      5[      5\      .@�5_      �� $��XLPilot��6   .��5`      "   XLPilot.TabControls.MainTab1  5a      �� 5b      $��Konfiguracja XL-e��6   .��5c      &    XLPilot.TabControls.XLConfigTab2  5d      �� 5e      $��Konfiguracja inne��6   .��5f      )   #XLPilot.TabControls.OtherConfigTab3  5g      5h      5i      5j      