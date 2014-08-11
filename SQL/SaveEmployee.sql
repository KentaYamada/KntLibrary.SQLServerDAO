use sampleDB
go
drop procedure SaveEmployee
go
create procedure SaveEmployee (
     @empno int
    ,@empname char(50)
    ,@sal int
    ,@hiredate datetime
    ,@deptno int
) as
begin tran

begin try
    update emp set
        empname = @empname
       ,sal = @sal
       ,deptno = @deptno
    where empno  =@empno
end try
begin catch
    rollback tran
end catch

if @@ROWCOUNT < 1
begin try
    insert into emp (
         empno
        ,empname
        ,sal
        ,hiredate
        ,deptno
    ) values (
         @empno
        ,@empname
        ,@sal
        ,@hiredate
        ,@deptno
    )
end try
begin catch
    rollback tran
end catch

--コミット
commit tran