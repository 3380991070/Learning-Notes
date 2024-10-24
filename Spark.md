

clear清屏

# 1.0概述

> ##### 分布式计算的核心 : 切分数据 减少数据规模
>
> 分布式集群中心化基础架构 : 主从架构(Master - Slave(奴隶))
>
> #### 基于内存的快速、通用、可扩展的大数据分析计算引擎

- ##### 内置模块

  - ##### Spark SQL 结构化数据

  - ##### Spark Streaming 实时计算

  - ##### Spark Mlib 机器学习

  - ##### Spark GraghX 图计算

# 1.1部署 Spark

> #### 将软件安装到什么位置
>
> ##### 部署Spark指的是Spark的程序逻辑在谁提供的资源中执行

- ##### 如果资源是当前单节点提供的，那么就称为单机模式 Local

- ##### 如果资源是当前多节点提供的，那么就称为分布式模式

  - 如果资源是由Yarn提供的，那么就称之为Yarn部署环境
  - 如果资源是由Spark提供的，那么就称之为Spark部署环境(Standalone)

- #### 解压缩

  - tar -zxvf sparkxxxxx.tgz
    - 解压
  - mv sparkxxxx/ spark-local
    - 重命名
  - mv spark-local /opt/module/
    - 移动

# 1.2Local模式

> ### 上面的解压缩操作就等于部署好了Local模式

- 运行官方提供的测试jar包
- --class 主类全类名
- --master local[*] 环境
- 10 运行次数
  - spark-submit --class org.apache.spark.examples.SparkPi --master local[*] ./jarbag位置 10

# 1.3Yarn模式

- 进入Spark的conf文件夹，将配置文件 spark-env.sh.template的后缀改为sh
  - mv spark-env.sh.template spark-env.sh
  - ////
  - cp spark-env.sh spark-env.sh
- 进入spark-env.sh进行编辑
- 添加内内容
  - 指定yarn配置文件路径
    - YARN_CONF_DIR=/opt/module/hadoop/etc/hadoop
- 启动Hadoop集群
- 执行examples
  - spark-submit --class org.apache.spark.examples.SparkPi --master yarn ../examples/jars/spark-examples_2.12-3.1.1.jar

## 1.3.1 三个进程

- ##### SparkSubmit 提交进程

- ##### YarnCoarseGrainedExecutorBackend 干活进程

- ##### ExecutorLauncher 管理进程(客户端模式)

- ##### ApplicationMaster (集群模式)

## 1.3.2 Yarn历史服务 history

> ##### spark-yarn/conf/spark-defaults.conf

- 添加内容
  - spark.eventLog.enabled	true
  - spark.eventLog.dir	hdfs://xxx:8082/directory
  - spark.yarn.historyServer.address=xxx:18080
  - spark.history.ui.port=18080

> ##### spark-yarn/conf/spark-env.sh

- 添加内容
  - exprot SPARK_HISTORY_OPTS="
  - -Dspark.history.ui.port=18080
  - -Dspark.history.fs.logDirectory=hdfs://xxx:8020/directory
  - -Dspark.history.retainedApplications=30"

```sh
export SPARK_HISTORY_OPTS="
-Dspark.history.ui.port=18080 
-Dspark.history.fs.logDirectory=hdfs://xxxx:端口/directory 
-Dspark.history.retainedApplications=30"
```

- 重启Spark历史服务
  - sbin/stop-history-server.sh
  - sbin/start-history-server.sh

## 1.3.3执行流程

> ##### RM资源申请
>
> NM nodeMaster

1. ##### 业务代码 -> subm

2. ##### submit -> context环境 ->Core -> SparkSubmit(JVM进程)(客户端) -> HDFS(运行环境全部上传)

3. ##### HDFS -> Yarn(Yarn下载文件)

4. ##### submit(Yarn)->RM(Yarn)

5. ##### RM->Driver(Yarn) NM

   1. ##### Driver(程序)

6. ##### RM->Executor(Yarn) NM

   1. ##### Executor(执行人) (slave)

7. ##### HDFS -> Driver -> Executor

> ### 将Driver放入集群内部 -> 集群模式

- #### Spark有yarn-client和yarn-cluster两种模式，主要区别在于 Driver程序的运行节点(默认client)

  - ##### client : Driver程序运行在客户端

  - ##### cluster : Driver程序运行在由ResourceManager启动的APPMaster

```sh
spark-submit 
--class org.apache.spark.examples.SparkPi 
--deploy-mode cluster
--master yarn ../examples/jars/spark-examples_2.12-3.1.1.jar
```

## 1.3.4模式对比

| 模式       | Spark安装机器数 | 需要启动的进程 | 属于   |
| ---------- | --------------- | -------------- | ------ |
| Local      | 1               | 无             | Spark  |
| Standalone | 3               | Master和Worke  | Spark  |
| Yarn       | 1               | Yarn和HDFS     | Hadoop |



# 2.0 SparkCore

## 2.1RDD概述

> #### RDD 弹性分布式数据集 是Spark中最基本的数据抽象
>
> ##### 分布式计算模型
>
> 它一定是个对象
>
> 它一定能完成对数据的一些操作(对数据结构做操作)
>
> 它一定封装了大量的方法和属性(计算逻辑)
>
> - 但是逻辑不能太复杂
> - 复杂的逻辑可以使用大量的简易的逻辑 组合在一起实现复杂的功能
>
> 它一定需要适合进行分布式处理(减小数据规模 并行计算)
>
> - RDD的操作类似字符串
>   - 字符串的功能是单点操作，功能一旦调用，就会马上执行
>   - RDD的功能是分布式操作，功能调用，但不会马上执行



- ##### 数据模型

  - #####  对事物属性的抽象，包含数据结构

- ##### 数据结构

  - ##### 采用特殊的结构组织和管理数据

    - ##### 数组

## 2.2 逻辑

1. ##### 数据 -> Driver(Master)

2. ##### Driver(内) -> RDD(封装逻辑) -> 拆分数据

3. ##### 打包数据Task(任务) [数据切片,逻辑] -> Slave



## 2.3数据处理的IO操作

> ## 多个RDD的功能如何组合在一起实现复杂逻辑

- IO : Input , Output

- ##### InputStream => 单个读取后直接执行操作 效率低

- ##### BufferdInputStream(new FileInputStream)

  - ##### 缓冲流包裹文件字节输入流

  - ##### Bufferd(缓冲区)本身无法读取文件

  - ##### 数据流入缓冲区，先不执行操作

  - ##### 缓冲区满了，执行操作

  > ### 字节流转字符流

- BufferedReader(new InputStreamReader(new FileInputStream))
- InputStreamReader => 转换流
  - 数据通过FileInputStream读取
  - 放入InputStreamReader缓冲区(转换) 3字节后转换为字符（Java编码使用）
  - 转换后数据放入字符缓冲流
  - 满后执行操作

> # 组合方式
>
> ##### BufferedReader(new InputStreamReader(new FileInputStream))
>
> ##### 设计模式 => 装饰者设计模式 : 组合功能 / 套娃
>
> ##### 将多个功能 装成了一个对象
>
> - 选择性的使用功能

> # RDD的处理方法和JavaIO流完全一样 
>
> # 也采用装饰者设计模式来实现功能的组合

# 3.0RDD编程

## 3.1创建RDD

> ### 方法

1. ##### 从集合中创建

2. ##### 从外部存储创建

3. ##### 从其他RDD创建

> ### 操作

1. 创建新的项目 => Maven项目
2. 添加依赖(对 spark-core)

```xml
<dependencies>
	<dependency>
		<groupId>org.apache.spark</>
		<artifactId>spark-core_版本(2.12)</> //scala语言版本
		<version>版本(3.3.1)</> //Spark版本
	</>
</>
```

> ### 代码实现

> ##### 构建Spark环境

```java
public class Spark_RDD_env
{
    public static void main(String[] args)
    {
        //创建Spark配置文件
        SparkConf conf = new SparkConf();
        //设置模式
        conf.setMaster("local");
        //设置程序名称
        conf.setAppName("Spark");
        //创建Spark运行环境
        JavaSparkContext sc = new JavaSparkContext(conf);
        sc.close();
    }
}
```

> ##### 构建RDD

```java
//利用Spark环境 对接内存数据源 并构建RDD
//parallelize => 并行
//内存中list必须存在
JavaRDD<String> javaRDD = sc.parallelize(list);
//收集数据
List<String> cool = javaRDD.collect();
//使用forEach结合Lamba表达式输出
cool.forEach(a -> System.out.println(a));
sc.close();
```

> 从文本文件中构建

```java
//读取文本文件构建rdd
JavaRDD<String> rdd = sc.textFile("src/main/java/Spark_RDD/Spark.txt");
//收集数据
List<String> data = rdd.collect();
//使用方法引用输出
data.forEach(System.out::println);
```

# 3.1RDD的理解

> - #### RDD数据模型存在泛型 只操作数据 不存，因为它是数据集
>
> - #### RDD类似于数据管道 用于对接(流转)数据
>
> - ##### RDD的组合
>
>   - ##### 两根管子对接

# 3.2RDD的文件分区

> Partition(分区)
>
> saveAsTextFile保存为文本文件
>
> 

- local环境中，分区数量和环境vcpu数量有关，但不推荐
- parallelize方法可以传递2个参数
  - 第一个参数表示对接的数据源集合
  - 第二个参数表示切片(分区)的数量
- 三种配置方式
  - 从配置对象中获取默认值 "spark.default.parallelism"
  - 使用方法参数
  - 使用配置参数

> File的分区
> 基于Hadoop的分区规则

- Hadoop切片规则
  - totalsize : 3byte => 文件总字节大小
  - goalsize : totalsize / min-part-num =>  文件总字节大小 / 最小分区数量 => 单个分区大小
  - part-num : 分区数量 => 文件总字节大小 / 单个分区大小
  - 如果part-num得到了余数，余数如果大于单个分区大小的10%那么建新区

- 在textFile的第二参数指定的是最小分区数



# 3.3RDD的内存数据源分配

> #### Spark分区数据的存储基本原则 : 平均分

- ##### (0 until 4) => [0,1,2,3]  取0到3不含4,这个4是指定的分区数

- ##### i * length / numSlices, ((i+1)*length)/numSlices

  - ##### length => 数据的长度

  - ##### numSlices => 切片的数量

- ##### (start,end) =>

  - ##### 开始点和结束点

# 3.4RDD的磁盘数据源分配

> #### Spark不支持文件操作，文件的操作都是由Hadoop完成的
>
> - Hadoop进行文件切片数量的计算核心文件数据存储计算规则不一样。
>   - 分区数量计算的时候，考虑的是尽可能的平均 => 
>     - 按字节来算
>   - 分区数据的存储是考虑业务数据的完整性 => 
>     - 不能按照字节来算 按照行来取
>     - 读取数据时，还需要考虑数据偏移量，偏移量从0开始
>     - 相同的偏移量不能重复读取

- ## 例

> 文件中有7字节
>
> 文件内容 => @回车
>
> {
>
> 1@@ => 012
>
> 2@@ => 345
>
> 3	=> 6
>
> }
>
> 第一个分区 [数据起始位,终止数据位]  数据3字节 把第二行的数据读取到了，直接整行读
>
> 【3byte】 =>[0,2] => 【1,2】
>
> 第二个分区 [数据起始位]  345已经被读过了，直接读下一个
>
> 【3byte】 =>[3,5] => 【3】
>
> 第三个分区 [数据起始位]（大于10%）全部被读完了 空
>
> 【1byte】 =>[6,6] => 【】

- # 结

> #### 在磁盘的数据读取上 Spark自己没有数据分配策略 使用的是hadoop的数据分配策略
>
> hadoop需要保证业务完整性 所以单个分区的内容即使溢出Spark的计算值，也要保证将整行数据放入
>
> 而被使用过的数据偏移量不能被重复使用，所以下一个数据分区读取的数据开始位就是上个块所读到的结束位
>
> 为什么会出现空分区=>
>
> ​	因为数据都被读取完了



> ### 在使用Spark读取磁盘数据时，需要分行，不然会出现数据倾斜
>
> #### 每一行 一个业务数据



# 3.5RDD功能

> ### String.trim只能去掉半角 不能去掉全角









<!--~~<u>*1.0 Spark环境搭建*</u>~~-->

## <!--~~<u>*1.1准备工作 Local-本地模式*</u>~~-->

- ##### <!--~~<u>*保证有JDK(java1.8)*</u>~~-->

- ##### <!--~~<u>*Windows(开发环境下)需要Scala Liunx不需要*</u>~~-->

- ##### <!--~~<u>*Spark安装包*</u>~~-->

### <!--~~<u>*1.2环境搭建*</u>~~-->

- <!--~~<u>*解压 Spark*</u>~~-->
- <!--~~<u>*更改名称 Spark*</u>~~-->
- <!--~~<u>*运行 bash bin/spark-shell.sh*</u>~~-->











# <!--<u>~~*环境搭建*~~</u>-->

## <!--<u>~~*本地环境搭建*~~</u>-->

- #### <!--<u>~~*准备工作*~~</u>-->

  - <!--<u>~~*安装JDK*~~</u>-->
  - <!--<u>~~*Scala安装包 -- Windows*~~</u>-->
  - <!--<u>~~*Spark安装包*~~</u>-->

<!--<u>~~**~~</u>-->	

- #### <!--<u>~~*操作*~~</u>-->

  - <!--<u>~~*将安装包上传到node1*~~</u>-->
  - <!--<u>~~*解压spark安装包*~~</u>-->

  ##### <!--<u>~~*更改文件权限*~~</u>-->

  <!--<u>~~*chown -R root 路径*~~</u>-->

  <!--<u>~~*chgrp -R root 路径*~~</u>-->

  - <!--<u>~~*更改长链接为软链*~~</u>-->

  <!--<u>~~*ln -s 长名 软连*~~</u>-->

- #### <!--<u>~~*解压操作*~~</u>-->

  - <!--<u>~~*tar -zxvf 压缩包 -C 目录*~~</u>-->

- #### <!--<u>~~*文件名称更改*~~</u>-->

  - <!--<u>~~*mv 文件名 更改后*~~</u>-->



- ### <!--<u>~~*文件提交与运行*~~</u>-->

  - <!--<u>~~*--master 谁来提供资源*~~</u>-->
  - <!--<u>~~*bin/spark-submit --class 类名 --master 运行环境(local本机) jar包路径*~~</u>--> 



- ### <!--<u>~~*Yarn环境配置*~~</u>-->

  - <!--<u>~~*/opt/module/spark-yarn/conf*~~</u>-->
  - <!--<u>~~*在spark-env.sh最后面添加 YARN_CONF_DIR=/opt/module/hadoop/etc/hadoop*~~</u>-->

# <!--<u>~~*Spark代码开发*~~</u>-->

- ## <!--<u>~~*工程创建 - Manve*~~</u>-->