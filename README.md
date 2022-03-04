# GodotPathGenerator
A godot plugin which can generate node path, so that you don't need to write literal string anymore.

Example:

![example](https://raw.githubusercontent.com/LiXin-Link/lixin-link.github.io/main/picture/gpgExample.png)

current editing scene is Main, and the Player node in it.
so that we can use IDE autocomplate than string.

Tips:
* only a scene with C# script can be generated path class, the class is inner GPG naming xxxPath.
* the default is in dir res://script/gpg defined in GodotPathGenerator class, you can modify the source to change it.

Todo:
* generate resource path(I think this is low usage)
* godot native signal name define(I really don't want to use literal string)

---

一个自动生成节点路径的godot插件，仅供C#使用（没有使用C#特有方法，可以很方便得移植到gdscript）

例子:

![例子](https://raw.githubusercontent.com/LiXin-Link/lixin-link.github.io/main/picture/gpgExample.png)

这样就可以使用IDE自动补全，而不是手写字符串

Tips:
* 只有有C#脚本的节点才会被生成
* 默认生成的文件路径是 res://script/gpg, 可以直接修改GodotPathGenerator类来改变

未来要做的:
* 生成资源文件的路径(这个用的可能比较少)
* godot自带信号的定义(可能和插件名没啥关系，但我就是想加进来)
