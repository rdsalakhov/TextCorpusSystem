   M S B A M L   `   `   `  ����?  ;View, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ViewView  5   	       View.MainAppWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   $localclr-namespace:View     5   	     � Title$  MainAppWindow��5   	   $��720.333��6   $��1024 ��60    � WindowStartupLocation$ CenterScreen=�5	       � 
ResizeMode$ CanMinimize=�6(   .��5
      � .+�5   
   o� $	��Top=�6   $
��Left=�6'   $
��1024��6B   $��18��6O   .@�5      m� $��Файл��6   .@�5      m� -   5         m�Name$ ImportTextButton��5      $��Импорт текста��65   6L   m� 5      -   5        $ OpenTextButton��5      $��Открыть текст��63   .@�5      �� -   5         ��Name$ TextsNamesComboBox��5       �� 
IsEditable .  $	��120��6N   ��6Z   �� 6�   5      m� 5      -   5        $ ExportTextButton��5      $!��Экспорт текста��65   {�.  6_   �� 5      m� 5      -   5        $ DeleteTextButton��5      $��Удалить текст��65   {�.  6k   m� 5      -   5        $ UpdateAnnotationButton��5      $)��#Обновить аннотацию��6;   {�.  6i   �� 5      m� 5      -   5        $ 
ExitButton��5      $��
Выход��6/   6>   5      m� 5      -   5        $ SearchMenuItem��5      $��
Поиск��6/   {�. .@�5      m� -	   5        $ CreateQueryButton��5      $!��Создать запрос��66   6N   5      m� 5      $��Аккаунт��6   .@�5      m� -
   5        $ ManageAccessButton��5      $)��#Управлять доступом��67   6S   m� 5      -   5        $ DeleteAccount��5      $#��Удалить аккаунт��62   6K   5      5      �� 5    
   -   5         ��Name$ HighlightedTextRichTB��5       $
��Left=�65   $��0,18,0,0q�6P   $	��793��6b   $��Calibri)�6n   $��16(�6�    �� 
LineHeight$ 6��6�    �� 
IsReadOnly . $
��Auto=�5!      $
��Auto=�5"      . �5#      /� .T�5$      J� .*�6   .*�5%      �� $+�%Файл -> Открыть текст��6   5&      5'      5(      �� 5)   
   -   5        $ HighligthedTagsRichTB��5)      $��
793,18,0,0q�65   $��Calibri)�6I   $��16(�6^   $ 6��6l    . $
��Auto=�5*      $
��Auto=�5+      . �5,      /� 5-      5.      5/      50      