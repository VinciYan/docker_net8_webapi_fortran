module test_module
    use iso_c_binding
    implicit none
contains
    ! 计算平方
    subroutine calculate_square(x, result) bind(C, name='calculate_square_')
        real(c_double), value, intent(in) :: x
        real(c_double), intent(out) :: result
        
        ! 添加打印语句进行调试
        print *, 'Fortran: input value = ', x
        result = x * x
        print *, 'Fortran: result = ', result
    end subroutine calculate_square

    ! 数组求和
    subroutine array_sum(arr, n, result) bind(C, name='array_sum_')
        integer(c_int), value, intent(in) :: n
        real(c_double), dimension(n), intent(in) :: arr
        real(c_double), intent(out) :: result
        integer :: i
        
        ! 添加打印语句进行调试
        print *, 'Fortran: array size = ', n
        print *, 'Fortran: input array = ', arr
        
        result = 0.0_c_double
        do i = 1, n
            result = result + arr(i)
        end do
        
        print *, 'Fortran: sum result = ', result
    end subroutine array_sum
    ! 二维数组求和
    subroutine matrix_sum(matrix, rows, cols, result) bind(C, name='matrix_sum_')
        integer(c_int), value, intent(in) :: rows, cols
        real(c_double), intent(in) :: matrix(rows, cols)
        real(c_double), intent(out) :: result
        integer :: i, j
        
        print *, 'Fortran: Matrix dimensions:', rows, 'x', cols
        print *, 'Fortran: Input matrix:'
        do i = 1, rows
            print *, matrix(i, :)
        end do
        
        result = 0.0_c_double
        do i = 1, rows
            do j = 1, cols
                result = result + matrix(i, j)
            end do
        end do
        
        print *, 'Fortran: Matrix sum:', result
    end subroutine matrix_sum

    ! 矩阵转置
    subroutine matrix_transpose(input, output, rows, cols) bind(C, name='matrix_transpose_')
        integer(c_int), value, intent(in) :: rows, cols
        real(c_double), intent(in) :: input(rows, cols)
        real(c_double), intent(out) :: output(cols, rows)
        integer :: i, j
        
        print *, 'Fortran: Transposing matrix:', rows, 'x', cols
        print *, 'Fortran: Input matrix:'
        do i = 1, rows
            print *, input(i, :)
        end do
        
        do i = 1, rows
            do j = 1, cols
                output(j, i) = input(i, j)
            end do
        end do
        
        print *, 'Fortran: Transposed matrix:'
        do i = 1, cols
            print *, output(i, :)
        end do
    end subroutine matrix_transpose
end module test_module