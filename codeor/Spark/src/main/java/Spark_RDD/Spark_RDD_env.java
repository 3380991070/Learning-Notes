package Spark_RDD;

import org.apache.spark.SparkConf;
import org.apache.spark.api.java.JavaSparkContext;

public class Spark_RDD_env
{
    public static void main(String[] args)
    {
        SparkConf conf = new SparkConf();
        conf.setMaster("local");
        conf.setAppName("Spark");
        JavaSparkContext sc = new JavaSparkContext(conf);

        sc.close();
    }
}