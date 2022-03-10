# GodotPathGenerator
A godot plugin which can generate node path and scene resource path, so that you don't need to write literal string anymore.(just for C#)  

*Write by C# but easy to transplant to gdscript.*

Example:

![example](https://raw.githubusercontent.com/LiXin-Link/GodotPathGenerator/main/.github/images/gpgExample.png)

![example2](https://raw.githubusercontent.com/LiXin-Link/GodotPathGenerator/main/.github/images/gpgResExample.png)

use IDE autocomplate than string.

Tips:
* the class will be generated when saving scene
* only a scene with C# script can be generated path class, the class is inner GPG naming xxxPath.
* the default generate dir `res://script/gpg` which defined in GodotPathGenerator class, you can modify the source to change it.

Todo:
* godot original signal name define(I really don't want to use literal string)

---

一个自动生成节点路径和场景资源路径的 godot 插件，这样就不用写字符串字面量了，仅供C#使用

*（没有使用C#特有方法，可以很方便得移植到gdscript）*

例子:

![example](https://raw.githubusercontent.com/LiXin-Link/GodotPathGenerator/main/.github/images/gpgExample.png)

![example2](https://raw.githubusercontent.com/LiXin-Link/GodotPathGenerator/main/.github/images/gpgResExample.png)

这样就可以使用IDE自动补全，而不是手写字符串

提示:
* 场景保存时会自动生成定义
* 只有有C#脚本的节点才会被生成，生成的类名是 xxxPath
* 默认生成的文件路径是 res://script/gpg, 可以在插件类 GodotPathGenerator 中修改

未来要做的:
* godot自带信号的定义(可能和插件名没啥关系，但我就是想加进来)
