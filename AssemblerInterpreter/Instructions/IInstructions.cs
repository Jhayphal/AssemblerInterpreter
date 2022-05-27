using AssemblerInterpreter.Processors;

namespace AssemblerInterpreter.Instructions
{
  public interface IInstructions<TProcessor, TRegister>
    where TProcessor : IProcessor<TRegister>
    where TRegister : struct
  {
    /// <summary>
    /// Commands list.
    /// </summary>
    Dictionary<string, Action<string[], TProcessor>> Instructions { get; }
    
    /// <summary>
    /// Names of commands that require changing label to address (compiler hint).
    /// </summary>
    List<string> RequireAddress { get; }

    /// <summary>
    /// Add the content of the register x with y (either an integer or the value of a register) and stores the result in x (i.e. register[x] += y).
    /// </summary>
    /// <remarks>Using: add x, y</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Add(string[] @params, TProcessor processor);

    /// <summary>
    /// Call to the subroutine by address. When a ret is found in a subroutine, the instruction pointer should return to the instruction next to this call command.
    /// </summary>
    /// <remarks>Using: call address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Call(string[] @params, TProcessor processor);

    /// <summary>
    /// Compares x (either an integer or the value of a register) and y (either an integer or the value of a register). The result is used in the conditional jumps (jne, je, jge, jg, jle and jl).
    /// </summary>
    /// <remarks>Using: cmp x, y</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Cmp(string[] @params, TProcessor processor);

    /// <summary>
    /// Decreases the content of the register x by one.
    /// </summary>
    /// <remarks>Using: dec x</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Dec(string[] @params, TProcessor processor);

    /// <summary>
    /// Same with integer division (i.e. register[x] /= y).
    /// </summary>
    /// <remarks>Using: div x, y</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Div(string[] @params, TProcessor processor);

    /// <summary>
    /// This instruction indicates that the program ends correctly, so the stored output is returned.
    /// </summary>
    /// <remarks>Using: end</remarks>
    /// <param name="_">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void End(string[] _, TProcessor processor);

    /// <summary>
    /// Increases the content of the register x by one.
    /// </summary>
    /// <remarks>Using: inc x</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Inc(string[] @params, TProcessor processor);

    /// <summary>
    /// Jump to the address if the values of the previous cmp command were equal.
    /// </summary>
    /// <remarks>Using: je address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Je(string[] @params, TProcessor processor);

    /// <summary>
    /// Jump to the address if x was greater than y in the previous cmp command.
    /// </summary>
    /// <remarks>Using: jg address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Jg(string[] @params, TProcessor processor);

    /// <summary>
    /// Jump to the address if x was greater or equal than y in the previous cmp command.
    /// </summary>
    /// <remarks>Using: jge address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Jge(string[] @params, TProcessor processor);

    /// <summary>
    /// Jump to the address if x was less than y in the previous cmp command.
    /// </summary>
    /// <remarks>Using: jl address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Jl(string[] @params, TProcessor processor);

    /// <summary>
    /// Jump to the address if x was less or equal than y in the previous cmp command.
    /// </summary>
    /// <remarks>Using: jle address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Jle(string[] @params, TProcessor processor);

    /// <summary>
    /// Jumps to an instruction by address
    /// </summary>
    /// <remarks>Using: jmp address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Jmp(string[] @params, TProcessor processor);

    /// <summary>
    /// Jump to the address if the values of the previous cmp command were not equal.
    /// </summary>
    /// <remarks>Using: jne address</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Jne(string[] @params, TProcessor processor);

    /// <summary>
    /// Jumps to an instruction y steps away (positive means forward, negative means backward, y can be a register or a constant), but only if x (a constant or a register) is not zero.
    /// </summary>
    /// <remarks>Using: jnz x y</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Jnz(string[] @params, TProcessor processor);
    
    /// <summary>
    /// Copies y (either a constant value or the content of a register) into register x.
    /// </summary>
    /// <remarks>Using: mov x y</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Mov(string[] @params, TProcessor processor);

    /// <summary>
    /// This instruction stores the output of the program. It may contain text strings (delimited by single quotes) and registers. The number of arguments isn't limited and will vary, depending on the program.
    /// </summary>
    /// <remarks>Using: msg 'Register: ', x</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Msg(string[] @params, TProcessor processor);

    /// <summary>
    /// Same with multiply (i.e. register[x] *= y).
    /// </summary>
    /// <remarks>Using: mul x, y</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Mul(string[] @params, TProcessor processor);

    /// <summary>
    /// When a ret is found in a subroutine, the instruction pointer should return to the instruction that called the current function.
    /// </summary>
    /// <remarks>Using: ret</remarks>
    /// <param name="_">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Ret(string[] _, TProcessor processor);
    
    /// <summary>
    /// Subtract y (either an integer or the value of a register) from the register x and stores the result in x (i.e. register[x] -= y).
    /// </summary>
    /// <remarks>Using: sub x, y</remarks>
    /// <param name="params">Parameters.</param>
    /// <param name="processor">Processr.</param>
    void Sub(string[] @params, TProcessor processor);
  }
}