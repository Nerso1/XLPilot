   M S B A M L   `   `   `  ���� B  >XLPilot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:XLPilot.UserControlsXLPilot.UserControls  &     XLPilot.UserControls.PilotButton   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ;userControls"clr-namespace:XLPilot.UserControls       � 	Resources  /   )XLPilot.UserControls.StringEmptyConverter   StringEmptyConverter �� 
TargetType  PilotButtonStyle �� Property �� Value �� 
TargetType ��Name �� CornerRadius ��Name ��Name	 �� RelativeSource
 �� AncestorType �� Path ��Name �� TextAlignment J� RenderTransformOrigin �� ScaleX �� ScaleY �� Triggers P� Property P� Value �� 
TargetName3  -System.Windows.Media.Effects.DropShadowEffect   
BlurRadius   ShadowDepth   Opacity �� Triggers �� Binding �� 	Converter �� Value��%+  &	        &	      7     �� " ��.���  � ��
 ���� ��  � ��
 ���� ��  � ��$ 10q���  � ��$ 16(���  � ��$ 2,2,2,2q���  � �� �� " ��.R��� $ 
MainBorder��
�������$ 4��._�� .+��� $ 	TilePanel��$��Center=�$��Center=�.+��� $ ButtonImage��$��Uniform=�$��60��$��60������ 	 �� $
 UserControlL�$ ImageSource��� $ ButtonTextBlock��
���   �$��12(�$ Center=�$��0,5,0,0q����� 	 �� $
 UserControlL�$ 
ButtonText��� $ AdminShieldImage��$��30��$��30��$:��4/XLPilot;component/Resources/Images/admin_shield.png��$��Right=�$��Bottom=�$��	0,0,10,30q�$��Uniform=�$ 0.5,0.5&�$y�	Collapsed=�z��� $ 1Q�$ 1Q� P�  �@J�IsMouseOver . . ��� $ 
MainBorder�� �@J�Effect   $ 10Q�$ 2Q�$	 0.5Q��� $ ButtonImage�� � z� �� $	 1.1Q�$	 1.1Q��� $ ButtonImage�� �@J�RenderTransformOrigin$ 0.5,0.5&��� $ AdminShieldImage�� � z� �� $	 1.1Q�$	 1.1Q��� $ ButtonTextBlock�� � ��$
 Bold"�P�  � �� . . ��� $ 
MainBorder�� Z�  �@J�Effect�� $ ButtonImage�� � z� �� $ 1Q�$ 1Q��� $ AdminShieldImage�� � z� �� $ 1Q�$ 1Q��� $ ButtonTextBlock�� � ��$ 12(��� $ ButtonTextBlock�� � ��$ Normal"��� $ ButtonTextBlock�� Z�  �@J�Effect ��  �� 	 �� $
 UserControlL�$ ToolTipText�8   	 False.a���  �@�ToolTip �� 	 �� $
 UserControlL�$ ToolTipText�.��� .+��� -    ��Name$ InternalButton��#��[ 