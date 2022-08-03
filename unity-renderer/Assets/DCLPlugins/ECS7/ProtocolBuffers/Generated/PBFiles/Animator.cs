// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Animator.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace DCL.ECSComponents {

  /// <summary>Holder for reflection information generated from Animator.proto</summary>
  public static partial class AnimatorReflection {

    #region Descriptor
    /// <summary>File descriptor for Animator.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static AnimatorReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg5BbmltYXRvci5wcm90bxIQZGVjZW50cmFsYW5kLmVjcyJACgpQQkFuaW1h",
            "dG9yEjIKBnN0YXRlcxgBIAMoCzIiLmRlY2VudHJhbGFuZC5lY3MuUEJBbmlt",
            "YXRpb25TdGF0ZSKCAQoQUEJBbmltYXRpb25TdGF0ZRIMCgRuYW1lGAEgASgJ",
            "EgwKBGNsaXAYAiABKAkSDwoHcGxheWluZxgDIAEoCBIOCgZ3ZWlnaHQYBCAB",
            "KAISDQoFc3BlZWQYBSABKAISDAoEbG9vcBgGIAEoCBIUCgxzaG91bGRfcmVz",
            "ZXQYByABKAhCFKoCEURDTC5FQ1NDb21wb25lbnRzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::DCL.ECSComponents.PBAnimator), global::DCL.ECSComponents.PBAnimator.Parser, new[]{ "States" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::DCL.ECSComponents.PBAnimationState), global::DCL.ECSComponents.PBAnimationState.Parser, new[]{ "Name", "Clip", "Playing", "Weight", "Speed", "Loop", "ShouldReset" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class PBAnimator : pb::IMessage<PBAnimator> {
    private static readonly pb::MessageParser<PBAnimator> _parser = new pb::MessageParser<PBAnimator>(() => new PBAnimator());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<PBAnimator> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::DCL.ECSComponents.AnimatorReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PBAnimator() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PBAnimator(PBAnimator other) : this() {
      states_ = other.states_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PBAnimator Clone() {
      return new PBAnimator(this);
    }

    /// <summary>Field number for the "states" field.</summary>
    public const int StatesFieldNumber = 1;
    private static readonly pb::FieldCodec<global::DCL.ECSComponents.PBAnimationState> _repeated_states_codec
        = pb::FieldCodec.ForMessage(10, global::DCL.ECSComponents.PBAnimationState.Parser);
    private readonly pbc::RepeatedField<global::DCL.ECSComponents.PBAnimationState> states_ = new pbc::RepeatedField<global::DCL.ECSComponents.PBAnimationState>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::DCL.ECSComponents.PBAnimationState> States {
      get { return states_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as PBAnimator);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(PBAnimator other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!states_.Equals(other.states_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= states_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      states_.WriteTo(output, _repeated_states_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += states_.CalculateSize(_repeated_states_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(PBAnimator other) {
      if (other == null) {
        return;
      }
      states_.Add(other.states_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            states_.AddEntriesFrom(input, _repeated_states_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class PBAnimationState : pb::IMessage<PBAnimationState> {
    private static readonly pb::MessageParser<PBAnimationState> _parser = new pb::MessageParser<PBAnimationState>(() => new PBAnimationState());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<PBAnimationState> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::DCL.ECSComponents.AnimatorReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PBAnimationState() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PBAnimationState(PBAnimationState other) : this() {
      name_ = other.name_;
      clip_ = other.clip_;
      playing_ = other.playing_;
      weight_ = other.weight_;
      speed_ = other.speed_;
      loop_ = other.loop_;
      shouldReset_ = other.shouldReset_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public PBAnimationState Clone() {
      return new PBAnimationState(this);
    }

    /// <summary>Field number for the "name" field.</summary>
    public const int NameFieldNumber = 1;
    private string name_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Name {
      get { return name_; }
      set {
        name_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "clip" field.</summary>
    public const int ClipFieldNumber = 2;
    private string clip_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Clip {
      get { return clip_; }
      set {
        clip_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "playing" field.</summary>
    public const int PlayingFieldNumber = 3;
    private bool playing_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Playing {
      get { return playing_; }
      set {
        playing_ = value;
      }
    }

    /// <summary>Field number for the "weight" field.</summary>
    public const int WeightFieldNumber = 4;
    private float weight_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Weight {
      get { return weight_; }
      set {
        weight_ = value;
      }
    }

    /// <summary>Field number for the "speed" field.</summary>
    public const int SpeedFieldNumber = 5;
    private float speed_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Speed {
      get { return speed_; }
      set {
        speed_ = value;
      }
    }

    /// <summary>Field number for the "loop" field.</summary>
    public const int LoopFieldNumber = 6;
    private bool loop_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Loop {
      get { return loop_; }
      set {
        loop_ = value;
      }
    }

    /// <summary>Field number for the "should_reset" field.</summary>
    public const int ShouldResetFieldNumber = 7;
    private bool shouldReset_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool ShouldReset {
      get { return shouldReset_; }
      set {
        shouldReset_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as PBAnimationState);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(PBAnimationState other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Name != other.Name) return false;
      if (Clip != other.Clip) return false;
      if (Playing != other.Playing) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Weight, other.Weight)) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Speed, other.Speed)) return false;
      if (Loop != other.Loop) return false;
      if (ShouldReset != other.ShouldReset) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Name.Length != 0) hash ^= Name.GetHashCode();
      if (Clip.Length != 0) hash ^= Clip.GetHashCode();
      if (Playing != false) hash ^= Playing.GetHashCode();
      if (Weight != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Weight);
      if (Speed != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Speed);
      if (Loop != false) hash ^= Loop.GetHashCode();
      if (ShouldReset != false) hash ^= ShouldReset.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Name.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Name);
      }
      if (Clip.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Clip);
      }
      if (Playing != false) {
        output.WriteRawTag(24);
        output.WriteBool(Playing);
      }
      if (Weight != 0F) {
        output.WriteRawTag(37);
        output.WriteFloat(Weight);
      }
      if (Speed != 0F) {
        output.WriteRawTag(45);
        output.WriteFloat(Speed);
      }
      if (Loop != false) {
        output.WriteRawTag(48);
        output.WriteBool(Loop);
      }
      if (ShouldReset != false) {
        output.WriteRawTag(56);
        output.WriteBool(ShouldReset);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Name.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Name);
      }
      if (Clip.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Clip);
      }
      if (Playing != false) {
        size += 1 + 1;
      }
      if (Weight != 0F) {
        size += 1 + 4;
      }
      if (Speed != 0F) {
        size += 1 + 4;
      }
      if (Loop != false) {
        size += 1 + 1;
      }
      if (ShouldReset != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(PBAnimationState other) {
      if (other == null) {
        return;
      }
      if (other.Name.Length != 0) {
        Name = other.Name;
      }
      if (other.Clip.Length != 0) {
        Clip = other.Clip;
      }
      if (other.Playing != false) {
        Playing = other.Playing;
      }
      if (other.Weight != 0F) {
        Weight = other.Weight;
      }
      if (other.Speed != 0F) {
        Speed = other.Speed;
      }
      if (other.Loop != false) {
        Loop = other.Loop;
      }
      if (other.ShouldReset != false) {
        ShouldReset = other.ShouldReset;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Name = input.ReadString();
            break;
          }
          case 18: {
            Clip = input.ReadString();
            break;
          }
          case 24: {
            Playing = input.ReadBool();
            break;
          }
          case 37: {
            Weight = input.ReadFloat();
            break;
          }
          case 45: {
            Speed = input.ReadFloat();
            break;
          }
          case 48: {
            Loop = input.ReadBool();
            break;
          }
          case 56: {
            ShouldReset = input.ReadBool();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
