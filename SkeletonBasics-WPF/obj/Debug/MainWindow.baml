   M S B A M L   `   `   `  ����d  `Microsoft.Maps.MapControl.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPFMicrosoft.Maps.MapControl.WPF  5   	    SkeletonBasics-WPF8   2Microsoft.Samples.Kinect.SkeletonBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   `mRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPF     5   	   -   5          � Title$  Skeleton Basics��5   	   $	��735��6!   $	��770��6.    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SquareCheckBox �� 
TargetType �� Orientation �Name	 ��Name
 ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%1  &	        &	 #     &	 F     '�� i     &	 �     7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5	   
   $ 	#ff52318f��64   �� 5
   
   $ 	#ff00BCF2��62   �� 5   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   � $ SquareCheckBoxChecked��6'   .+�5   &   �� $	 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5   &   $	 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5   #   � 5   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�5   &   �� $	 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5   &   $	 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   5   #   �� 5   "   $
 SquareCheckBoxText��6,   #��z��6H   $
 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   5      5       5      P� 5        �@d�	IsChecked$ falseY�6;   . �5!   "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5"   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5#      P� 5$       �@J�IsMouseOver . . �5%   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5&   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5'   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5(   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5)   "   8  $ SquareCheckBoxText��6h    � ��5*      5,      5.      5/      50      .��52      � -   5        $��
layoutGrid��52      $��	10 0 10 0q�6    � RowDefinitions 53   
   �� 54      $
��Auto �6   $
��67.2��6*   �� 55      �� 56      $
��Auto �6   �� 57      $
��Auto �6   .+�59   
   '   !Microsoft.Maps.MapControl.WPF.Map  -   5          Name$ myMap��59      $��4��6   +   %Microsoft.Maps.MapControl.WPF.MapCore   CredentialsProviderT   NMicrosoft.Maps.MapControl.WPF.Design.ApplicationIdCredentialsProviderConverter$r l0JbzNXxaqJam9uOChVzc~OL4ubOxhmU4QXRlhrkhI3g~Aj-KxTfYO5Xfec0xQ0nXJVLPJeYUeFSIRckkxz6CC0v-A9x_hTobSKLks9Rb6g3F 5:      ]� 5<   
   $��0��6   $��0,0,0.2,20.2q�6!   .+�5?      �� $��1��6   $��Images\Status.png��6$   $
��None=�6?   $��Center=�6N   $��0 0 0 5q�6k   5@      (� 5B   
   $��1��6    (� Stretch$ Uniform=�6   $��Center=�61   .��5C      �� -   5        $��Image��5C      $	��640��6!   $	��480��6-   5D      �� 5F   
   -   5        $��checkBoxSeatedMode��5F   �   $��2��6   #��[ 6    $��Seated Mode��6H   $��24��6^   $
��Left=�6j   $��Center=�6�   $
��0,10q�6�   $	��122��6  �� 5G   
   -   5        $��	statusBar��5G   ?   $��3��6   $��Stretch=�6!   $��Bottom=�6P   
�������#��[  6~   $��26��6�   $��0,0,0.2,-0.4q�6�   .@�5H      �� $��0 0 0 10q�6   .��5I      �� -   5        $��statusBarText��5I      $��-1 0 0 0q�61   20Click 'Seated' to change skeletal pipeline type!6u   5J      5K      5P      5S      