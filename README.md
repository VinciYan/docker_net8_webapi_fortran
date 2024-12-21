```
cd fortran
gfortran -c -fPIC test.f90 -o test.o
gfortran -shared test.o -o libtest.so

cp libtest.so ../
```

## Docker

```
docker build -t docker_net8_webapi_fortran:1.0 .

docker run -d -p 9986:5000 --name test_docker_net8_webapi_fortran docker_net8_webapi_fortran:1.0
```